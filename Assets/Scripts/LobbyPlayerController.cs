using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class LobbyPlayerController : CommonController {

    private PlayerManagerController playerManager;
    private PlayerConfig playerConfig;
    private GameObject playerObject;
    private Renderer[] playerRenderers;
    private Renderer[] playerHats;
    
    private int cIndex = 0;  // color index
    private int hIndex = -1;  // hat index
    private int pIndex;  // player index

    private void Start() {
        pIndex = GetComponent<PlayerInput>().playerIndex;
        playerManager = PlayerManagerController.Instance;
        playerConfig = playerManager.getPlayerConfig(pIndex);
        playerObject = playerConfig.PlayerObject;
        playerConfig.PlayerColor = cIndex;

        if (playerObject) {
            playerRenderers = playerObject.transform.Find("Model").GetComponentsInChildren<Renderer>();
            playerHats = playerObject.transform.Find("Hats").GetComponentsInChildren<Renderer>();
        } else {
            playerRenderers = new Renderer[0];
            playerHats = new Renderer[0];
        }
    }

    private void OnUp() {
        // change monkey color
        if (playerConfig != null && !playerConfig.IsReady) {
            if (cIndex == 0) cIndex = playerManager.playerColors.Count - 1;
            else cIndex--;

            ChangePlayerColor();
        }
    }

    private void OnDown() {
        // change monkey color
        if (playerConfig != null && !playerConfig.IsReady) {
            if (cIndex == playerManager.playerColors.Count - 1) cIndex = 0;
            else cIndex++;

            ChangePlayerColor();
        }
    }

    private void OnLeft() {
        // change hat
        if (playerConfig != null && !playerConfig.IsReady) {
            if (hIndex != -1) {
                playerHats[hIndex].enabled = false;
                hIndex--;
            } else {
                hIndex = playerHats.Length - 1;
            }

            ChangePlayerHat();
        }
    }

    private void OnRight() {
        // change hat
        if (playerConfig != null && !playerConfig.IsReady) {
            if (hIndex != -1) {
                playerHats[hIndex].enabled = false;
            }
            
            if (hIndex == playerHats.Length - 1) hIndex = -1;
            else hIndex++;

            ChangePlayerHat();
        }
    }

    private void OnReady() {
        if (playerManager && playerConfig != null) playerManager.ReadyPlayer(pIndex);
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

        playerConfig.PlayerColor = cIndex;
    }

    private void ChangePlayerHat() {
        // enable renderer for the corresponding hat
        if (hIndex != -1) playerHats[hIndex].enabled = true;

        playerConfig.PlayerHat = hIndex;
    }

    public void BindPlayer() {
        // switch to game action map
        GetComponent<PlayerInput>().SwitchCurrentActionMap("Default");

        // unbind lobby monkey and bind to player monkey
        playerConfig.PlayerObject = gameObject;

        // change appearance of player monkey based on selected customisation
        Renderer[] playerRenderers = gameObject.transform.Find("Model").GetComponentsInChildren<Renderer>();

        foreach (Renderer ren in playerRenderers) {
            ren.material = playerManager.playerColors[cIndex];
        }

        // if no hat selected, then don't enable any
        if (hIndex != -1) {
            Renderer[] playerHats = gameObject.transform.Find("Hats").GetComponentsInChildren<Renderer>();
            playerHats[hIndex].enabled = true;
        }
    }
}
