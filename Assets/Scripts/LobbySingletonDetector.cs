using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LobbySingletonDetector : MonoBehaviour {
    public GameObject playerConfigManager;

    private PlayerManagerController playerManager;

    private void Awake() {
        if (!PlayerManagerController.Instance) {
            playerConfigManager.SetActive(true);
        }
    }
}
