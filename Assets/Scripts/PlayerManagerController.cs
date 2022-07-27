using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class PlayerManagerController : MonoBehaviour {

    public List<GameObject> playerCards;
    public List<Material> playerColors;

    private List<PlayerConfig> playerConfigs;
    private int MinPlayers = 2;

    public static PlayerManagerController Instance { get; private set; }

    private void Awake() {
        if (!Instance) {
            Instance = this;
            DontDestroyOnLoad(Instance);
            playerConfigs = new List<PlayerConfig>();
        }
    }

    public List<PlayerConfig> getPlayerConfigs() {
        return playerConfigs;
    }

    public PlayerConfig getPlayerConfig(int pIndex) {
        return playerConfigs[pIndex];
    }

    public void ReadyPlayer(int index) {
        Debug.Log("Ready Player " + index);

        playerConfigs[index].IsReady = true;

        // only start game if there are at least 2 players, and all players are ready
        if (playerConfigs.Count >= MinPlayers && playerConfigs.All(p => p.IsReady == true)) {

            // TODO: randomly select the string name of an available map
            GameObject[] playerData = GameObject.FindGameObjectsWithTag("PlayerData");

            foreach (GameObject player in playerData) {
                player.GetComponent<LobbyPlayerController>().BindPlayer();
            }

            SceneManager.LoadScene("Beach");
        }
    }

    public void HandlePlayerJoin(PlayerInput pInput) {
        int index = pInput.playerIndex;

        if (!playerConfigs.Any(p => p.PlayerIndex == index)) {
            pInput.transform.SetParent(transform);

            // enable corresponding player card
            playerCards[index].SetActive(true);

            GameObject playerObject = playerCards[index].transform.Find("Lobby Monkey").gameObject;

            playerConfigs.Add(new PlayerConfig(pInput, playerObject));
        }
    }

    public void HandlePlayerLeft(PlayerInput pInput) {
        // [KIV] TODO: handle players leaving (not sure how Unity.PlayerInput auto deals with it in terms of the playerIndex)
    }
}


public class PlayerConfig {

    public PlayerInput Input { get; set; }
    public int PlayerIndex { get; set; }

    public GameObject PlayerObject { get; set; }
    public bool IsReady { get; set; }

    public PlayerConfig(PlayerInput pInput, GameObject playerObject) {
        Input = pInput;
        PlayerIndex = pInput.playerIndex;
        PlayerObject = playerObject;
    }
}
