using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    private GameObject enemy;
    private AIMoveLogic enemyScript;

    private void Start()
    {
        enemy = GameObject.FindGameObjectWithTag("Enemy");
        enemyScript = enemy.GetComponent<AIMoveLogic>();
        Debug.Log("Fist Check");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == enemy)
        {
            Debug.Log("Right hand found");
            enemyScript.TakeHit();
        }
    }
}
