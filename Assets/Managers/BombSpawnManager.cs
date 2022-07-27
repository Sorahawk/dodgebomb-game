using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombSpawnManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> bombList = new List<GameObject>();
    public GameObject spawnGrid;

    private int z_index;
    private int x_index;

    private Transform gridChild;

    private Vector3 spawnPoint;
    private int bombIndex;
     
    void Start(){
        count = transform.childCount;
        spawnpoints = new Transform[count];
        for(int i = 0; i < count; i++){
            spawnpoints[i] = transform.GetChild(i);
        }
        
        InvokeRepeating("spawnBombs", 1, 3);
        InvokeRepeating("spawnBombs", 1, 5);
        InvokeRepeating("spawnBombs", 1, 2);
    }
    
    void spawnBombs()
    {
        z_index = Random.Range(0, spawnGrid.transform.childCount);
        gridChild = spawnGrid.transform.GetChild(z_index);

        x_index = Random.Range(0, gridChild.childCount);
        spawnPoint = gridChild.transform.GetChild(x_index).position;

        // random spawns in fixed positions
        bombIndex = Random.Range(0, bombList.Count);
        Instantiate(bombList[bombIndex], new Vector3(spawnPoint.x, spawnPoint.y+1.0f, spawnPoint.z), bombList[bombIndex].transform.rotation);

        // random spawn in random positions
        // float _xAxis = Random.Range(-18,18);
        // float _zAxis = Random.Range(-6,25);
        // Instantiate(bombList[bombIndex], new Vector3(_xAxis, 2.0f, _zAxis), bombList[bombIndex].transform.rotation);
    }
}
