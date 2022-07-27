using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class LobbyPlayerController : CommonController {

    private PlayerManagerController playerManager;
    private PlayerConfig playerConfig;
    private GameObject playerObject;
    private Renderer[] playerRenderers;

    private int cIndex = 0;
    private bool isReady;
    private int pIndex;

    private void Start() {
        pIndex = GetComponent<PlayerInput>().playerIndex;
        playerManager = PlayerManagerController.Instance;
        playerConfig = playerManager.getPlayerConfig(pIndex);
        playerObject = playerConfig.PlayerObject;

        playerRenderers = playerObject.GetComponentsInChildren<Renderer>();
    }

    private void OnUp() {
        Debug.Log("Up button clicked");
    }

    private void OnDown() {
        Debug.Log("Down button clicked");
    }

    private void OnLeft() {
        if (cIndex == 0) cIndex = playerManager.playerColors.Count - 1;
        else cIndex--;

        ChangePlayerColor();
    }

    private void OnRight() {
        if (cIndex == playerManager.playerColors.Count - 1) cIndex = 0;
        else cIndex++;

        ChangePlayerColor();
    }

    private void OnReady() {
        playerManager.ReadyPlayer(pIndex);
    }

    private void OnBack() {
        // TODO: Implement back button from lobby to menu
        Debug.Log("Back button clicked");
    }

    private void ChangePlayerColor() {
        // assign new material to all child mesh renderers
        foreach (Renderer ren in playerRenderers) {
            ren.material = playerManager.playerColors[cIndex];
        }
    }

    public void BindPlayer() {
        // switch to the correct input component
        GetComponent<PlayerInput>().enabled = false;
        playerObject.GetComponent<PlayerInput>().enabled = true;

        // disable kinematics
        playerObject.GetComponent<Rigidbody>().isKinematic = false;

        // bind to the persistent PlayerData object
        playerObject.transform.SetParent(gameObject.transform);
    }
}
