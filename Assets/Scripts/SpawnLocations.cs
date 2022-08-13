using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLocations : MonoBehaviour
{
    public VectorListVariable vectorListVariable;
    private void Awake() {
        vectorListVariable.resetVectorList();
        foreach (Transform tr in transform) {
            if (tr.tag == "SpawnPoint") {
                vectorListVariable.addVector(tr.position);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
