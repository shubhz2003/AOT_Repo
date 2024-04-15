using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    private GameObject enemy;
   

    private void Start()
    {
        enemy = GameObject.FindGameObjectWithTag("Enemy");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Neck") 
        {
            Debug.Log("Neck hit");
            enemy.gameObject.GetComponent<AIMoveLogic>().Death();

        }
    }
}
