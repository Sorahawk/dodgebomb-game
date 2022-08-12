using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupSpawnManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> powerupList = new List<GameObject>();
    public GameObject spawnGrid;

    private int z_index;
    private int x_index;

    private Transform gridChild;

    private Vector3 spawnPoint;
    private int powerupIndex;
     
    void Start(){
        InvokeRepeating("spawnPowerups", 15, 25);
    }
    
    void spawnPowerups()
    {
        print("spawning powerup");
        z_index = Random.Range(0, spawnGrid.transform.childCount);
        gridChild = spawnGrid.transform.GetChild(z_index);

        x_index = Random.Range(0, gridChild.childCount);
        spawnPoint = gridChild.transform.GetChild(x_index).position;

        // random spawns in fixed positions
        powerupIndex = Random.Range(0, powerupList.Count);
        Instantiate(powerupList[powerupIndex], new Vector3(spawnPoint.x, spawnPoint.y + 1f, spawnPoint.z), powerupList[powerupIndex].transform.rotation);

        // random spawn in random positions
        // float _xAxis = Random.Range(-18,18);
        // float _zAxis = Random.Range(-6,25);
        // Instantiate(bombList[bombIndex], new Vector3(_xAxis, 2.0f, _zAxis), bombList[bombIndex].transform.rotation);
    }
}
