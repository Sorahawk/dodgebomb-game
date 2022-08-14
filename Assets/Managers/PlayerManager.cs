using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class PlayerManager : MonoBehaviour {

    public List<GameObject> playerObjects;
    public List<Material> playerColors;
    public GameObject actualMonkeyPrefab;
    public RoundManager roundManager;
    public int minPlayers = 2;

    private List<PlayerConfig> playerConfigs;

    public static PlayerManager Instance { get; private set; }

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

        if (!playerConfigs[index].IsReady) {
            playerConfigs[index].IsReady = true;

            playerObjects[index].transform.Find("Ready").gameObject.SetActive(true); // ready
            playerObjects[index].transform.Find("Arrows").gameObject.SetActive(false); // arrows
        }

        else {
            playerConfigs[index].IsReady = false;

            playerObjects[index].transform.Find("Ready").gameObject.SetActive(false); // ready
            playerObjects[index].transform.Find("Arrows").gameObject.SetActive(true); // arrows
        }

        // only start game if there are at least 2 players, and all players are ready
        if (playerConfigs.Count >= minPlayers && playerConfigs.All(p => p.IsReady == true)) {

            // disable player input manager joining so new controllers can't be added in halfway
            GetComponent<PlayerInputManager>().DisableJoining();

            // wipe playerObjects list
            playerObjects = new List<GameObject>();

            LobbyController[] lobbyControllers = transform.GetComponentsInChildren<LobbyController>();

            foreach(LobbyController controller in lobbyControllers) {
                // prepare player monkey
                controller.BindPlayer();

                // add each player monkey object to the playerObjects list
                playerObjects.Add(controller.gameObject);
            }

            StartCoroutine(roundManager.StartNewRound());
        }
    }

    public void HandlePlayerJoin(PlayerInput pInput) {
        int index = pInput.playerIndex;

        if (!playerConfigs.Any(p => p.PlayerIndex == index)) {
            pInput.transform.SetParent(transform);

            // enable corresponding player card
            playerObjects[index].SetActive(true);
            playerObjects[index].transform.Find("Name").GetComponent<UnityEngine.UI.Text>().text = "Player " + (index + 1);
            
            GameObject playerObject = playerObjects[index].transform.Find("Lobby Monkey").gameObject;
            playerConfigs.Add(new PlayerConfig(pInput, playerObject));
        }
    }

    public void HandlePlayerLeft(PlayerInput pInput) {
        // [KIV] TODO: handle players leaving (not sure how Unity.PlayerInput auto deals with it in terms of the playerIndex)
    }
}


public class PlayerConfig {

    public PlayerInput Input { get; set; }
    public GameObject PlayerObject { get; set; }

    public int PlayerIndex { get; set; }
    public int PlayerColor { get; set; }
    public int PlayerHat { get; set; }
    public bool IsReady { get; set; }

    public PlayerConfig(PlayerInput pInput, GameObject playerObject) {
        Input = pInput;
        PlayerIndex = pInput.playerIndex;
        PlayerObject = playerObject;
        PlayerHat = -1;
        IsReady = false;
    }
}
