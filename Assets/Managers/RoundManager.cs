using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System;


public class RoundManager : MonoBehaviour {
    // public PlayerInput playerInput;
    public GameConstants gameConstants;
    public GameObject playerConfigManager;
    public VectorListVariable VectorListVariable;
    public FloatVariable TimerFloatVariable;
    public FloatVariable roundStartingCountdownFloatVariable;
    public PlayerVariable[] playerVarList;
    public BoolVariable pausedBoolVariable;
    
    // timer
    private bool roundStarting = false;
    private bool timerIsRunning = false;
    private bool roundEnded = false;
    private float timeRemaining;
    private float startingTimer;

    private Transform[] spawnpoints;
    private int count;
    private int[] mapSpawned;
    private int roundNumber;
    private int previousSpawnPoint = -1;

    // scene loading
    private int numberOfNonPlayableScenes; 
    private int maxMapIndex; // Number of playable maps
    private bool isSummaryShown = false;

    public static RoundManager Instance { get; private set; }

    public int[] getRoundNumbers() {
        return new int[] {roundNumber, maxMapIndex};
    }

    private void Awake() {
        if (!Instance) {
            Instance = this;
        }
        numberOfNonPlayableScenes = gameConstants.numberOfNonPlayableScenes;
        maxMapIndex = SceneManager.sceneCountInBuildSettings - numberOfNonPlayableScenes;
        
        ResetAllRoundInfo();
        TimerFloatVariable.SetValue(gameConstants.roundDuration);

        // reset all player scores to 0
        foreach (PlayerVariable playerVar in playerVarList) {
            playerVar.SetScore(0);
        }
    }

    void Start() {
        // spawn points
        count = transform.childCount;
        spawnpoints = new Transform[count];
        for(int i = 0; i < count; i++){
            spawnpoints[i] = transform.GetChild(i);
        }

        // print(InputSystem.devices);

        // check how many players and spawn them in the first six spawn points
        // foreach (Transform coordinate in spawnpoints){
        //     Instantiate(myPrefab, new Vector3(coordinate.position.x, coordinate.position.y, coordinate.position.z), Quaternion.identity);
        // }

        // disable controls
    }

    void Update() {
        if (roundStarting) {
            StartingCountdown();
        }

        else if (timerIsRunning) {
            RoundTimerCountdown();
        }

        else if (roundEnded) {
            if (!isSummaryShown) {
                // show the round summary screen
                StartCoroutine(LoadPostRound());
                isSummaryShown = true;
            }
        }

        else if (roundNumber == maxMapIndex) {
            // show the game summary screen
        }
    }

    private IEnumerator LoadPostRound() {
        // wait for scene to finish loading
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("PostRound");

        while (!asyncLoad.isDone) yield return null;

        // wait for scene to be loaded before setting active
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("PostRound"));
    }

    void StartingCountdown() {
        if (startingTimer > 0) startingTimer -= Time.deltaTime;
        else {
            roundStarting = false;
            timerIsRunning = true;
            // enable controls

        }

        roundStartingCountdownFloatVariable.SetValue(startingTimer);
    }

    void RoundTimerCountdown() {
        if (timeRemaining > 0) timeRemaining -= Time.deltaTime;
        else {
            Debug.Log("Time has run out!");
            timeRemaining = 0;
            timerIsRunning = false;
            roundEnded = true;
        }

        TimerFloatVariable.SetValue(timeRemaining);
    }

    // On starting new round, increment round number, reset round timer and change to a new map.
    public IEnumerator StartNewRound() {
        // based on Scene Index under File -> Build Settings
        // ignore index 0 - 2 (Start, Instructions, Lobby, PostRound)

        int randomMap = UnityEngine.Random.Range(0, maxMapIndex) + numberOfNonPlayableScenes;
        bool exists = Array.Exists( mapSpawned, element => element == randomMap);

        while (exists) {
            randomMap = UnityEngine.Random.Range(0, maxMapIndex)+numberOfNonPlayableScenes;
        }

        // wait for scene to finish loading
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(randomMap);

        while (!asyncLoad.isDone) {
            yield return null;
        }

        // wait for scene to be loaded before setting active
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(randomMap));
        SpawnAllPlayers();
        timeRemaining = gameConstants.roundDuration;
        startingTimer = gameConstants.startingCountdown;
        roundStarting=true;  // starts the countdown before the round begins
        roundEnded = false;  // resets this from previous round
        roundNumber +=1;
        Debug.Log(roundNumber);
    }

    // To be called when returning to main menu or restart pressed.
    public void ResetAllRoundInfo(){
        mapSpawned = new int[maxMapIndex];
        roundNumber = 0;
    }

    public IEnumerator playerDeathRespawn(GameObject player) {
        Vector3[] vectorList = VectorListVariable.VectorList;
        yield return new WaitForSeconds(gameConstants.respawnTimer);
        int randomSpawnPoint = UnityEngine.Random.Range(0, 6);
        while(randomSpawnPoint == previousSpawnPoint){
            randomSpawnPoint = UnityEngine.Random.Range(0, 6);
        }
        player.transform.position = vectorList[randomSpawnPoint];
        player.GetComponent<PlayerController>().RevivePlayer();
    }

    public void SpawnAllPlayers() {
        // set the transform position of all monkeys
        int counter = 0;
        Vector3[] vectorList = VectorListVariable.VectorList;
        foreach (Transform tr in playerConfigManager.transform) {
            if (tr.tag == "Player") {
                tr.position = vectorList[counter];
                counter+=1;
            }
        }
    }


}
