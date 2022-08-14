using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameEndUIManager : MonoBehaviour {
    public List<GameObject> playerCards;
    public PlayerVariable[] playerVarList;
    public BoolVariable QuitorRestartBooleanVariable;

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
        }
    }

    public void ButtonRematch() {
        QuitorRestartBooleanVariable.SetValue(true);
        SceneManager.LoadScene("Lobby");
    }
}
