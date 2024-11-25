using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;

public class AcievementManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject achievementItemPrefab; // Prefab for each achievement
    [SerializeField] private Transform achievementsContent;    // ScrollView Content


    private const string AchievementsKey = "Achievements";

    // Dictionary to hold achievements locally
    private Dictionary<string, Achievement> achievements = new Dictionary<string, Achievement>();

    void Start()
    {
        FetchAchievementsFromPlayFab();
    }

    // Fetch achievements from PlayFab
    //void FetchAchievementsFromPlayFab()
    //{
    //    PlayFabClientAPI.GetUserData(new GetUserDataRequest(), result =>
    //    {
    //        if (result.Data != null && result.Data.ContainsKey(AchievementsKey))
    //        {
    //            string json = result.Data[AchievementsKey].Value;

    //            achievements = JsonUtility.FromJson<AchievementList>(json).ToDictionary();
    //        }
    //        else
    //        {
    //            Debug.Log("No achievements data found. Initializing default list.");
    //            InitializeDefaultAchievements();
    //        }

    //        PopulateAchievementsUI();
    //    },
    //    error => Debug.LogError("Failed to fetch achievements: " + error.GenerateErrorReport()));
    //}


    void FetchAchievementsFromPlayFab()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            result =>
            {
                if (result.Data != null && result.Data.ContainsKey(AchievementsKey))
                {
                    string jsonData = result.Data[AchievementsKey].Value;
                    Debug.Log($"Achievements fetched: {jsonData}");

                    achievements = JsonUtility.FromJson<AchievementList>(jsonData).ToDictionary();
                }
                else
                {
                    Debug.Log("No achievements found. Initializing default achievements.");
                    InitializeDefaultAchievements();
                }

                PopulateAchievementsUI();
            },
            error =>
            {
                Debug.LogError($"Failed to fetch achievements: {error.GenerateErrorReport()}");
            });
    }


    // Initialize a default achievements list if none exist
    void InitializeDefaultAchievements()
    {
        achievements = new Dictionary<string, Achievement>
        {
            { "StartGame", new Achievement("StartGame", "Start Game", "Start Your First Game", false) },
            { "FinishGame", new Achievement("FinishGame", "Finish Game", "Finish Your First Game", false) },
            { "FirstKill", new Achievement("FirstKill", "First Blood", "Kill your first enemy", false) },
            { "Collector", new Achievement("Collector", "Treasure Hunter", "Collect 10 items", false) },
            { "Explorer", new Achievement("Explorer", "World Traveler", "Visit all regions", false) },
            { "Sharpshooter", new Achievement("Sharpshooter", "Precision Aim", "Score 10 headshots", false) }
        };

        SaveAchievementsToPlayFab();
    }

    // Save achievements to PlayFab
    void SaveAchievementsToPlayFab()
    {
        var json = JsonUtility.ToJson(new AchievementList(achievements));
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string> { { AchievementsKey, json } }
        },
        result => Debug.Log("Achievements data saved successfully."),
        error => Debug.LogError("Failed to save achievements: " + error.GenerateErrorReport()));
    }

    // Populate ScrollView with achievements
    void PopulateAchievementsUI()
    {
        foreach (Transform child in achievementsContent.transform)
        {
            Destroy(child.gameObject); // Clear existing items
        }

        foreach (var achievement in achievements.Values)
        {
            GameObject item = Instantiate(achievementItemPrefab, achievementsContent.transform);

            TMP_Text titleText = item.transform.Find("AchievementTitle").GetComponent<TMP_Text>();
            TMP_Text descriptionText = item.transform.Find("AchievementDescription").GetComponent<TMP_Text>();

            titleText.text = achievement.Title;
            descriptionText.text = achievement.Description;

            if (achievement.IsCompleted)
            {
                titleText.color = Color.green;
                descriptionText.color = Color.green;
            }
            else
            {
                titleText.color = Color.black;
                descriptionText.color = Color.black;
            }
        }
    }
}

// Data structures for achievements
[System.Serializable]
public class Achievement
{
    public string ID;
    public string Title;
    public string Description;
    public bool IsCompleted;

    public Achievement(string id, string title, string description, bool isCompleted)
    {
        ID = id;
        Title = title;
        Description = description;
        IsCompleted = isCompleted;
    }
}

// List for serialization
[System.Serializable]
public class AchievementList
{
    public List<Achievement> List;

    public AchievementList(Dictionary<string, Achievement> dictionary)
    {
        List = new List<Achievement>(dictionary.Values);
    }

    public Dictionary<string, Achievement> ToDictionary()
    {
        var dict = new Dictionary<string, Achievement>();
        foreach (var achievement in List)
        {
            dict[achievement.ID] = achievement;
        }
        return dict;
    }
}
