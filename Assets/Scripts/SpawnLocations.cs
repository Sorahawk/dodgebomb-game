using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpawnLocations : MonoBehaviour {
    public VectorListVariable vectorListVariable;
    public BoolVariable mapLoadedBoolVariable;

    private void Awake() {
        mapLoadedBoolVariable.SetValue(false);
        vectorListVariable.resetVectorList();
        foreach (Transform tr in transform) {
            if (tr.tag == "SpawnPoint") {
                vectorListVariable.addVector(tr.position);
            }
        }
        mapLoadedBoolVariable.SetValue(true);
    }
}
