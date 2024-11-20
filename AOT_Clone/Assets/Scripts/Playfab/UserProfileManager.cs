using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UserProfileManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_Text profileInfoText;
    [SerializeField] private TMP_Text loginMessage;
    [SerializeField] private GameObject loginPage;
    [SerializeField] private GameObject profilePage;
    [SerializeField] private GameObject loginButton;

    private string currentUsername;

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

        // Show profile page with user info
        profileInfoText.text = $"Welcome, {currentUsername}!";
        ShowProfilePage();
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

    // Logs out the user and resets the UI.
    public void Logout()
    {
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
