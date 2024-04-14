using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFIstCheck : MonoBehaviour
{
    private GameObject titan;
    private AIMoveLogic enemyScript;

   

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
             titan = other.gameObject;
             enemyScript = titan.GetComponent<AIMoveLogic>();
             if(enemyScript != null ) 
             {
                enemyScript.Death();
             }
        }
    }
}
