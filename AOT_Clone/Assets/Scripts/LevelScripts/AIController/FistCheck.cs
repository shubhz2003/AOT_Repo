using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FistCheck : MonoBehaviour
{
    private GameObject player;
    private TestMove playerScript;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<TestMove>();
        Debug.Log("Fist Check");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            Debug.Log("Right hand found");
            playerScript.DecreaseHealth();
        }
    }
}


