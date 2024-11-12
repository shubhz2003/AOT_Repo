using PlayFab;
using PlayFab.ClientModels;
using UnityEditor.PackageManager;
using UnityEngine;
using TMPro;

public class PlayFabAuth : MonoBehaviour
{
    public TMP_Text messageText;
    public string username = "GuestUser";  // Default username

    // Start is called before the first frame update
    void Start()
    {
        LoginAsGuest();
    }

    public void LoginAsGuest()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = System.Guid.NewGuid().ToString(),
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Logged in successfully");
        messageText.text = "Logged in successfully as Guest!";
        // Load user data to check username and playtime
        GetUserData();
    }

    void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("Login Failed: " + error.GenerateErrorReport());
        messageText.text = "Login Failed!";
    }

    public void GetUserData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnUserDataReceived, OnError);
    }

    void OnUserDataReceived(GetUserDataResult result)
    {
        if (result.Data != null && result.Data.ContainsKey("Username"))
        {
            username = result.Data["Username"].Value;
        }
        else
        {
            messageText.text = "Welcome, " + username + "!";
        }
    }

    void OnError(PlayFabError error)
    {
        Debug.LogError("Error: " + error.GenerateErrorReport());
    }
}
