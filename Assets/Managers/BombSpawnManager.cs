using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombSpawnManager : MonoBehaviour
{
    [SerializeField]
     private GameObject bombPrefab;
     private int spawnIndex;
     private Transform[] spawnpoints;
     private Vector3 spawnPos;
     private int count;
     
     void Start(){
         count = transform.childCount;
         spawnpoints = new Transform[count];
         for(int i = 0; i < count; i++){
             spawnpoints[i] = transform.GetChild(i);
         }
         
         InvokeRepeating("spawnBombs", 1, 5);
     }
     
     void spawnBombs(){
         spawnIndex = Random.Range(0, count);
         float _xAxis = Random.Range(-18,18);
         float _zAxis = Random.Range(-6,25);
         
         Instantiate(bombPrefab, spawnpoints[spawnIndex].position, bombPrefab.transform.rotation);
        //  Instantiate(bombPrefab, new Vector3(_xAxis, 2.0f, _zAxis), bombPrefab.transform.rotation);
     }
}
