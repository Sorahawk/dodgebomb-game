using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HUDManager : MonoBehaviour {
    public List<GameObject> playerCards;

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
            playerManager.playerObjects[i].GetComponent<PlayerController>().setHUDManager(this);
        }
    }

    // Update is called once per frame
    void Update() {

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
}
