using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class PlayerConfigManager : MonoBehaviour {

    public List<GameObject> playerCards;

    [SerializeField]
    private int MinPlayers = 2;
    private List<PlayerConfig> playerConfigs;

    public static PlayerConfigManager Instance { get; private set; }

    private void Awake() {
        if (Instance) Debug.Log("PlayerConfigManager already instantiated.");
        else {
            Instance = this;
            playerConfigs = new List<PlayerConfig>();
        }
    }

    public void SetPlayerColor(int index, Material color) {
        playerConfigs[index].PlayerMaterial = color;
    }

    public void ReadyPlayer(int index) {
        playerConfigs[index].IsReady = true;

        if (playerConfigs.Count >= MinPlayers && playerConfigs.All(p => p.IsReady == true)) {
            SceneManager.LoadScene("Beach");
        }
    }

    public void HandlePlayerJoin(PlayerInput pInput) {
        int index = pInput.playerIndex;
        Debug.Log("Player Joined " + index);

        if (!playerConfigs.Any(p => p.PlayerIndex == index)) {
            pInput.transform.SetParent(transform);
            playerConfigs.Add(new PlayerConfig(pInput));
        }

        if (!playerCards[index].activeInHierarchy) playerCards[index].SetActive(true);
    }

    public void HandlePlayerLeft(PlayerInput pInput) {
        // handle players leaving?       
    }
}


public class PlayerConfig {

    public PlayerConfig(PlayerInput pInput) {
        PlayerIndex = pInput.playerIndex;
        Input = pInput;
    }

    public PlayerInput Input { get; set; }
    public Material PlayerMaterial { get; set; }

    public int PlayerIndex { get; set; }
    public bool IsReady { get; set; }
}
