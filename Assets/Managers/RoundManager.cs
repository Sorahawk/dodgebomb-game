using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System;


public class RoundManager : MonoBehaviour {
    public GameConstants gameConstants;
    public GameObject playerConfigManager;
    public VectorListVariable VectorListVariable;
    public FloatVariable TimerFloatVariable;
    public FloatVariable roundStartingCountdownFloatVariable;
    public PlayerVariable[] playerVarList;
    
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
    private int numberOfNonPlayableScenes; 
    private int maxMapIndex; // Number of playable maps
    private int previousSpawnPoint = -1;

    public static RoundManager Instance { get; private set; }

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

    // Start is called before the first frame update
    void Start()
    {
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

    // Update is called once per frame
    void Update()
    {

        if (roundStarting)
        {
            StartingCountdown();
        }
        else if (timerIsRunning)
        {
            RoundTimerCountdown();
        }
        else if (roundEnded)
        {
            // show the round summary screen
        }
        else if (roundNumber == maxMapIndex)
        {
            // show the game summary screen
        }
    }

    void StartingCountdown()
    {
        if (startingTimer > 0)
            {
                startingTimer -= Time.deltaTime;
            }
            else
            {
                roundStarting = false;
                timerIsRunning = true;
                // enable controls
            }
    }

    void RoundTimerCountdown()
    {
        if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                Debug.Log("Time has run out!");
                timeRemaining = 0;
                timerIsRunning = false;
                roundEnded = true;
            }
    }

    // On starting new round, increment round number, reset round timer and change to a new map.
    public IEnumerator StartNewRound(int previousMapIndex = -1) {
        // based on Scene Index under File -> Build Settings
        // ignore index 0 - 2 (Start, Instructions and Lobby)
        
        
        int randomMap = UnityEngine.Random.Range(0, maxMapIndex)+numberOfNonPlayableScenes;
        bool exists = Array.Exists( mapSpawned, element => element == randomMap);
        while(exists){
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
