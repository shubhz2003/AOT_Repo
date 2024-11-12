using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class UserProfileManager : MonoBehaviour
{
    public TMP_InputField usernameInputField;
    public TMP_Text profileInfoText;
    public TMP_Text loginMessage;
    public GameObject loginPage;
    public GameObject profilePage;

    // Start is called before the first frame update
    void Start()
    {
        ShowLoginPage();
    }

    // Method to initiate login or registration
    public void LoginOrRegister()
    {
        string newUsername = usernameInputField.text;
        if (!string.IsNullOrEmpty(newUsername))
        {
            CheckUsernameUniqueness(newUsername);
        }
        else
        {
            loginMessage.text = "Please enter a valid username.";
        }
    }

    // Check if the username is unique by searching PlayFab
    void CheckUsernameUniqueness(string username)
    {
        var request = new GetAccountInfoRequest
        {
            Username = username
        };

        PlayFabClientAPI.GetAccountInfo(
            request,
            OnUsernameExists,
            error =>
            {
                //Debug.LogError("Error checking username uniqueness: " + error.GenerateErrorReport());
                RegisterNewUser(username);
            }
        );
    }

    // If username exists, login the user; otherwise, create a new profile
    void OnUsernameExists(GetAccountInfoResult result)
    {
        // User exists, log in using the username
        //loginMessage.text = "Welcome back!";
        profileInfoText.text = "Welcome back!" ;
        ShowProfilePage();
        GetProfileData();
    }

    // If username is unique, register a new user
    void RegisterNewUser(string username)
    {
        // Create a new account for this username
        var request = new RegisterPlayFabUserRequest
        {
            Username = username,
            DisplayName = username,
            Password = "default_password", // Default password (for example purposes)
            RequireBothUsernameAndEmail = false
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, OnUserRegistered, OnError);
    }

    void OnUserRegistered(RegisterPlayFabUserResult result)
    {
        //loginMessage.text = "Account created! Welcome, " + result.Username;
        profileInfoText.text = "Welcome, " + result.Username + "!";
        ShowProfilePage();
        SaveUsername(result.Username);
    }

    // Save the username as display name in PlayFab
    public void SaveUsername(string username)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = username
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdated, OnError);
    }

    void OnDisplayNameUpdated(UpdateUserTitleDisplayNameResult result)
    {
        profileInfoText.text = "Username saved!";
        GetProfileData();
    }

    //void OnUsernameUpdated(UpdateUserDataResult result)
    //{
    //    profileInfoText.text = "Username saved!";
    //    GetProfileData();
    //}

    public void GetProfileData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnProfileDataReceived, OnError);
    }

    void OnProfileDataReceived(GetUserDataResult result)
    {
        string username = result.Data.ContainsKey("Username") ? result.Data["Username"].Value : "Guest";
        profileInfoText.text = $"Username: {username}\nHours Played: 0"; // Placeholder for hours played
    }

    void OnError(PlayFabError error)
    {
        Debug.LogError("Error: " + error.GenerateErrorReport());
        loginMessage.text = "Error: " + error.ErrorMessage;
    }

    public void ShowLoginPage()
    {
        loginPage.SetActive(true);
        profilePage.SetActive(false);
    }

    public void ShowProfilePage()
    {
        profilePage.SetActive(true);
        loginPage.SetActive(false);
        GetProfileData();
    }
}
