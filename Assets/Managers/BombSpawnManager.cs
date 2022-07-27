using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombSpawnManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> bombList = new List<GameObject>();
    private int spawnIndex;
    private int bombIndex;
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

        // random spawns in fixed positions
        bombIndex = Random.Range(0, bombList.Count);
        Instantiate(bombList[bombIndex], spawnpoints[spawnIndex].position, bombList[bombIndex].transform.rotation);

        // random spawn in random positions
        // float _xAxis = Random.Range(-18,18);
        // float _zAxis = Random.Range(-6,25);
        // Instantiate(bombList[bombIndex], new Vector3(_xAxis, 2.0f, _zAxis), bombList[bombIndex].transform.rotation);
    }
}
