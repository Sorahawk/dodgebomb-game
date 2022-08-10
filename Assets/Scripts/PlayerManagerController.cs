using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class PlayerManagerController : MonoBehaviour {

    public List<GameObject> playerCards;
    public List<Material> playerColors;
    public GameObject actualMonkeyPrefab;
    public int MinPlayers = 2;

    private List<PlayerConfig> playerConfigs;

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

        if (!playerConfigs[index].IsReady) {
            playerConfigs[index].IsReady = true;

            playerCards[index].transform.Find("Ready").gameObject.SetActive(true); // ready
            playerCards[index].transform.Find("Arrows").gameObject.SetActive(false); // arrows
        }

        else {
            playerConfigs[index].IsReady = false;

            playerCards[index].transform.Find("Ready").gameObject.SetActive(false); // ready
            playerCards[index].transform.Find("Arrows").gameObject.SetActive(true); // arrows
        }

        // only start game if there are at least 2 players, and all players are ready
        if (playerConfigs.Count >= MinPlayers && playerConfigs.All(p => p.IsReady == true)) {

            LobbyPlayerController[] lobbyControllers = transform.GetComponentsInChildren<LobbyPlayerController>();

            foreach(LobbyPlayerController controller in lobbyControllers) {
                // bind new monkey to PlayerObject
                controller.BindPlayer();
            }

            StartCoroutine(LoadNewMap());
        }
    }

    // based on Scene Index under File -> Build Settings
    // ignore index 0 - 2 (Start, Instructions and Lobby)
    public IEnumerator LoadNewMap(int previousMapIndex = -1) {
        int numberOfNonPlayableScenes = 3;

        int maxMapIndex = SceneManager.sceneCountInBuildSettings;
        int randomMap = Random.Range(numberOfNonPlayableScenes, maxMapIndex);

        // wait for scene to finish loading
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(randomMap);

        while (!asyncLoad.isDone) {
            yield return null;
        }

        // wait for scene to be loaded before setting active
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(randomMap));
        SpawnAllPlayers();
    }

    public void SpawnAllPlayers() {
        // set the transform position of all monkeys
        foreach (Transform tr in transform) {
            if (tr.tag == "Player") {
                tr.position = Vector3.zero;
            }
        }
    }

    public void HandlePlayerJoin(PlayerInput pInput) {
        int index = pInput.playerIndex;

        if (!playerConfigs.Any(p => p.PlayerIndex == index)) {
            pInput.transform.SetParent(transform);

            // enable corresponding player card
            playerCards[index].SetActive(true);

            GameObject playerObject = playerCards[index].transform.Find("Lobby Monkey").gameObject;
            playerCards[index].transform.Find("Name").GetComponent<UnityEngine.UI.Text>().text = "Player " + (index+1);
            
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
