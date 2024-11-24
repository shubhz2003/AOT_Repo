using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class UserProfileManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_Text profileInfoText;
    [SerializeField] private TMP_Text loginMessage;
    [SerializeField] private TMP_Text totalHoursPlayedText;
    [SerializeField] private GameObject loginPage;
    [SerializeField] private GameObject profilePage;
    [SerializeField] private GameObject loginButton;

    private string currentUsername;
    private DateTime sessionStartTime; // Track session start time
    private const string TotalHoursKey = "TotalHoursPlayed"; // Key for PlayFab PlayerData
    private const string LastLoginTimeKey = "LastLoginTime"; // Key for last login time

    // Start is called before the first frame update
    void Start()
    {
        ShowLoginPage();
        loginButton.SetActive(false);
    }


    // Triggered by the login button. Logs in or registers the user based on the username input.
    public void LoginOrRegister()
    {
        string username = usernameInputField.text.Trim();

        if (string.IsNullOrEmpty(username) || username.Length < 3 || username.Length > 25)
        {
            loginMessage.text = "Username must be between 3 and 25 characters.";
            return;
        }

        currentUsername = username.ToLower(); // Save the username locally
        sessionStartTime = DateTime.UtcNow;
        LoginWithUsername(currentUsername);
    }

    // Attempts to log in with the given username. If the user doesn't exist, it creates a new account.
    void LoginWithUsername(string username)
    {
        var loginRequest = new LoginWithCustomIDRequest
        {
            CustomId = username,
            CreateAccount = true // Creates account if username doesn't exist
        };

        PlayFabClientAPI.LoginWithCustomID(loginRequest, OnLoginSuccess, OnLoginFailure);
    }

    void OnLoginSuccess(LoginResult result)
    {
        Debug.Log($"Successfully logged in as {currentUsername}");

        // Save the username for new accounts
        SaveUsername(currentUsername);

        // Fetch player data to display time played
        FetchPlayerData();

        // Show profile page with user info
        //profileInfoText.text = $"Welcome, {currentUsername}!";
        //ShowProfilePage();
    }

    void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("Login failed: " + error.GenerateErrorReport());
        loginMessage.text = "Login failed. Please try again.";
    }

    // Saves the username as the display name in PlayFab.
    void SaveUsername(string username)
    {
        var displayNameRequest = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = username
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(displayNameRequest,
            result =>
            {
                Debug.Log($"Display name updated to: {result.DisplayName}");
                profileInfoText.text = $"Welcome, {result.DisplayName}!";
            },
            error =>
            {
                Debug.LogError("Failed to update display name: " + error.GenerateErrorReport());
            });
    }

    void FetchPlayerData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), result =>
        {
            double totalHours = 0;
            DateTime? lastLoginTime = null;

            // Parse TotalHoursPlayed
            if (result.Data != null && result.Data.ContainsKey(TotalHoursKey))
            {
                double.TryParse(result.Data[TotalHoursKey].Value, out totalHours);
            }

            // Parse LastLoginTime
            if (result.Data != null && result.Data.ContainsKey(LastLoginTimeKey))
            {
                long ticks;
                if (long.TryParse(result.Data[LastLoginTimeKey].Value, out ticks))
                {
                    lastLoginTime = new DateTime(ticks);
                }
            }

            // Calculate hours from the previous session
            if (lastLoginTime.HasValue)
            {
                TimeSpan sessionTime = sessionStartTime - lastLoginTime.Value;
                totalHours += sessionTime.TotalHours;
            }

            // Format total time played in hours and minutes
            TimeSpan totalTimePlayed = TimeSpan.FromHours(totalHours);
            totalHoursPlayedText.text = $"Total Time Played: {totalTimePlayed.Hours}h {totalTimePlayed.Minutes}m";

            // Save updated data
            SavePlayerData(totalHours);

            // Show the profile page
            ShowProfilePage();
        },
        error =>
        {
            Debug.LogError("Failed to fetch player data: " + error.GenerateErrorReport());
        });
    }

    void SavePlayerData(double totalHours)
    {
        var userData = new UpdateUserDataRequest
        {
            Data = new System.Collections.Generic.Dictionary<string, string>
            {
                { TotalHoursKey, totalHours.ToString() },
                { LastLoginTimeKey, DateTime.UtcNow.Ticks.ToString() } // Save current time
            }
        };

        PlayFabClientAPI.UpdateUserData(userData,
        result => Debug.Log("Player data updated successfully."),
        error => Debug.LogError("Failed to update player data: " + error.GenerateErrorReport()));
    }

    // Logs out the user and resets the UI.
    public void Logout()
    {
        // Calculate session time
        TimeSpan sessionTime = DateTime.UtcNow - sessionStartTime;

        // Fetch and update total hours played
        double currentTotalHours = 0;
        if (double.TryParse(totalHoursPlayedText.text.Replace("Total Hours Played: ", ""), out currentTotalHours))
        {
            currentTotalHours += sessionTime.TotalHours;
        }

        SavePlayerData(currentTotalHours);

        currentUsername = null;
        usernameInputField.text = string.Empty;
        loginMessage.text = "Please log in.";
        profileInfoText.text = string.Empty;

        ShowLoginPage();
    }

    // Shows the login page.
    void ShowLoginPage()
    {
        loginPage.SetActive(true);
        profilePage.SetActive(false);
    }

    // Shows the profile page.
    void ShowProfilePage()
    {
        loginPage.SetActive(false);
        profilePage.SetActive(true);
    }
}
