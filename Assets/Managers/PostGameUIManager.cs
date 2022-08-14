using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PostGameUIManager : MonoBehaviour {
    public List<GameObject> playerCards;
    public PlayerVariable[] playerVarList;

    private PlayerManager playerManager;
    private List<PlayerConfig> playerConfigs;

    void Start() {
        playerManager = PlayerManager.Instance;
        playerConfigs = playerManager.getPlayerConfigs();

        // enable the appropriate number of cards based on how many players
        for (int i = 0; i < playerConfigs.Count; i++) {
            PlayerConfig playerConfig = playerConfigs[i];
            GameObject playerCard = playerCards[i];

            playerCard.SetActive(true);

            // set monkey player number
            Text playerText = playerCard.transform.Find("Player Text").GetComponent<Text>();
            playerText.text = "Player " + (i + 1);

            Transform monkeyTransform = playerCard.transform.Find("Lobby Monkey");

            // set monkey color
            Renderer[] headRenderer = monkeyTransform.Find("Model").GetComponentsInChildren<Renderer>();
            foreach (Renderer ren in headRenderer) {
                ren.material = playerManager.playerColors[playerConfig.PlayerColor];
            }

            // set monkey hat
            Renderer[] hatRenderers = monkeyTransform.Find("Hats").GetComponentsInChildren<Renderer>();
            int hatIndex = playerConfig.PlayerHat;

            if (hatIndex != -1) {
                hatRenderers[hatIndex].enabled = true;
            }

            // bind playerCard to that player's PlayerController script
            //playerManager.playerObjects[i].GetComponent<PlayerController>().setHUDManager(this);
        }
    }

    void Update() {
        // update player scores every frame by checking playerVariable scores
        for (int i = 0; i < playerConfigs.Count; i++) {
            int playerScore = playerVarList[i].Score;

            // update score text
            Text scoreText = playerCards[i].transform.Find("Kills Text").GetComponent<Text>();
            scoreText.text = "Kills " + playerScore;
        }
    }

    public void ShowPowerup(int playerIndex, int powerupIndex) {
        GameObject powerupIcon = playerCards[playerIndex].transform.Find("Powerups").GetChild(powerupIndex).gameObject;
        powerupIcon.SetActive(true);
    }

    public void HidePowerup(int playerIndex) {
        Transform powerupIcons = playerCards[playerIndex].transform.Find("Powerups");

        for (int i = 0; i < 4; i++) {
            powerupIcons.GetChild(i).gameObject.SetActive(false);
        }
    }

    public IEnumerator ActivateDashCooldown(int playerIndex) {
        Renderer[] dashIcons = playerCards[playerIndex].transform.Find("Dash").GetComponentsInChildren<Renderer>();

        Renderer runningIcon = dashIcons[0];

        runningIcon.enabled = false;

        // 3 second countdown
        for (int i = 1; i < 4; i++) {
            dashIcons[i].enabled = true;
            yield return new WaitForSeconds(1);
            dashIcons[i].enabled = false;
        }

        runningIcon.enabled = true;
    }
}
