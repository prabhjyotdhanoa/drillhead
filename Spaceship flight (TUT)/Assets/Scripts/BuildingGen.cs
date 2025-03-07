﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGen : MonoBehaviour
{

    public float spawnRange;
    public float amountToSpawn;
    private Vector3 spawnPoint;
    public GameObject tallbuilding;
    public float startSafeRange;
    private List<GameObject> objectsToPlace = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < amountToSpawn; i++)
        {
            PickSpawnPoint();

            //pick new spawn point if too close to player start
            while (Vector3.Distance(spawnPoint, Vector3.zero) < startSafeRange)
            {
                PickSpawnPoint();
            }

            objectsToPlace.Add(Instantiate(tallbuilding, spawnPoint, Quaternion.Euler(0, 0, 0)));
            objectsToPlace[i].transform.parent = this.transform;
        }

        tallbuilding.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PickSpawnPoint()
    {
        spawnPoint = new Vector3(
            Random.Range(-1f,1f),
            0f,
            Random.Range(-1f, 1f));

        if(spawnPoint.magnitude > 1)
        {
            spawnPoint.Normalize();
        }

        spawnPoint *= spawnRange;
    }
}
