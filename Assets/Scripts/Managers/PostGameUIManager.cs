using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PostGameUIManager : MonoBehaviour {
    public List<GameObject> playerCards;
    public PlayerVariable[] playerVarList;

    private PlayerManager playerManager;
    private RoundManager roundManager;
    private List<PlayerConfig> playerConfigs;

    void Start() {
        playerManager = PlayerManager.Instance;
        playerConfigs = playerManager.getPlayerConfigs();

        roundManager = playerManager.transform.Find("RoundManager").GetComponent<RoundManager>();

        // set round number
        int[] roundNums = roundManager.getRoundNumbers();
        Text roundText = transform.Find("Round Text").Find("Text").GetComponent<Text>();
        roundText.text = "Round " + roundNums[0] + " / " + roundNums[1];

        // calculate player ranks by consolidating all scores
        // TODO: player ranking (if not just disable it)

        // enable the appropriate number of cards based on how many players
        for (int i = 0; i < playerConfigs.Count; i++) {
            PlayerConfig playerConfig = playerConfigs[i];
            GameObject playerCard = playerCards[i];

            playerCard.SetActive(true);

            Transform monkeyTransform = playerCard.transform.Find("Image").Find("Lobby Monkey");

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

            // set player score
        }
    }

    public void ButtonNextRound() {
        roundManager.StartNewRound();
    }
}
