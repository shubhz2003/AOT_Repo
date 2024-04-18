using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitanManager : MonoBehaviour
{
    public GameObject titanPrefab;
    public int numberOfTitans = 5;
    public GameObject[] titans;

    void Start()
    {
        for (int i = 0; i < numberOfTitans; i++)
        {
            Vector3 spawnPosition = new Vector3(
                Random.Range(-100f, 100f),
                0,
                Random.Range(-100f, 100f)
            );

            Instantiate(titanPrefab, spawnPosition, Quaternion.identity);
        }
    }

   
}



