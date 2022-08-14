using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LobbySingletonDetector : MonoBehaviour {
    public GameObject playerConfigManager;

    private PlayerManager playerManager;

    private void Awake() {
        if (!PlayerManager.Instance) {
            playerConfigManager.SetActive(true);
        }
    }
}
