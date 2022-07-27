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
    // private bool isPlayerReady = false;

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

        if (playerConfigs[index].IsReady == false){
            playerConfigs[index].IsReady = true;

            // ui element for ready
            playerCards[index].transform.GetChild(0).gameObject.SetActive(false);
            playerCards[index].transform.GetChild(1).gameObject.SetActive(true);
            playerCards[index].transform.GetChild(3).gameObject.SetActive(true);
        }
        else{
            playerConfigs[index].IsReady = false;

            // ui element for ready
            playerCards[index].transform.GetChild(0).gameObject.SetActive(true);
            playerCards[index].transform.GetChild(1).gameObject.SetActive(false);
            playerCards[index].transform.GetChild(3).gameObject.SetActive(false);
        }



        // only start game if there are at least 2 players, and all players are ready
        if (playerConfigs.Count >= MinPlayers && playerConfigs.All(p => p.IsReady == true)) {

            // unbind lobby monkey from each PlayerData object
            GameObject[] playerData = GameObject.FindGameObjectsWithTag("PlayerData");

            foreach (GameObject player in playerData) {
                player.GetComponent<LobbyPlayerController>().BindPlayer();
            }

            LoadNewMap();
        }
    }

    // based on Scene Index under File -> Build Settings
    // ignore index 0 and 1 (Start and Lobby)
    public void LoadNewMap(int previousMapIndex = -1) {
        int maxMapIndex = SceneManager.sceneCountInBuildSettings - 1;
        int randomMap = Random.Range(2, maxMapIndex);

        SceneManager.LoadScene(randomMap);
        SpawnAllPlayers();
    }

    public void SpawnAllPlayers() {
        GameObject[] playerData = GameObject.FindGameObjectsWithTag("PlayerData");

        // spawn monkeys
        // bind each input to the respective playerindex
        foreach (GameObject player in playerData) {
            player.GetComponent<PlayerInput>();
        }
    }

    public void HandlePlayerJoin(PlayerInput pInput) {
        int index = pInput.playerIndex;

        if (!playerConfigs.Any(p => p.PlayerIndex == index)) {
            pInput.transform.SetParent(transform);

            // enable corresponding player card
            playerCards[index].SetActive(true);

            GameObject playerObject = playerCards[index].transform.Find("Lobby Monkey").gameObject;
            playerCards[index].transform.GetChild(2).GetComponent<UnityEngine.UI.Text>().text = "Player " + (index+1);
            

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
    public bool IsReady { get; set; }

    public PlayerConfig(PlayerInput pInput, GameObject playerObject) {
        Input = pInput;
        PlayerIndex = pInput.playerIndex;
        PlayerObject = playerObject;
    }
}
