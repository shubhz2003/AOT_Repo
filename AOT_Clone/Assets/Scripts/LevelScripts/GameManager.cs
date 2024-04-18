using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using static UnityEditor.Timeline.TimelinePlaybackControls;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    //public static GameManager Instance;
    //public int health;
    public static GameManager Instance;

    public AIMoveLogic[] titans;
    [SerializeField] private TextMeshProUGUI winText;
    [SerializeField] private TextMeshProUGUI titanText;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI timerText;

    public GameObject gameOverPanel;
    private Image panelImage;
    private GameObject Player;
    private TestMove moveLogic;
    private float timer;


    //[SerializeField] private TextMeshProUGUI healthGUI;
    void Start()
    {
        winText.enabled = false;
        if (Instance == null)
        {
            Instance = this;
        }
        panelImage = gameOverPanel.GetComponent<Image>();
        Player = GameObject.FindGameObjectWithTag("Player");
        moveLogic = Player.GetComponent<TestMove>();
        timer = 0f;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        //titans = GameObject.FindGameObjectsWithTag("Enemy");
        titans = FindObjectsOfType<AIMoveLogic>();
        Debug.LogWarning($"Titans number: {titans.Length}");
        timerText.text = "Time: " + timer;
        //timerText.text = timer.ToString();
        if (titans.Length == 0)
        {
            panelImage.enabled = true;
            gameOverText.color = Color.red;
            gameOverText.enabled = true;
            gameOverText.text = "ENEMY SLAIN!!";
            //winText.enabled = true;
        }
        else if (moveLogic.Health <= 0)
        {
            panelImage.enabled = true;
            gameOverText.color = Color.red;
            gameOverText.enabled = true;
            gameOverText.text = "YOU DIED!!";
        }
        else if( timer >= 300)
        {
            panelImage.enabled = true;
            gameOverText.color = Color.red;
            gameOverText.enabled = true;
            gameOverText.text = "Time out!!";
        }

        titanText.text = "Titans: " + titans.Length;

        
    }
}
