using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class RoundManager : MonoBehaviour {
    // public PlayerInput playerInput;
    public GameConstants gameConstants;
    public GameObject playerConfigManager;
    public VectorListVariable VectorListVariable;
    public FloatVariable TimerFloatVariable;
    public FloatVariable roundStartingCountdownFloatVariable;
    public PlayerVariable[] playerVarList;
    public BoolVariable pausedBoolVariable;
    public BoolVariable mapLoadedBoolVariable;
    
    // timer
    private bool roundStarting = false;
    private bool timerIsRunning = false;
    private bool roundEnded = false;
    private float timeRemaining;
    private float startingTimer;

    private Transform[] spawnpoints;
    private int count;
    private int roundNumber;
    private int previousSpawnPoint = -1;

    // scene loading
    [SerializeField]
    private List<int> mapIndexList;
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

        mapIndexList = new List<int>();

        for (int i = numberOfNonPlayableScenes; i < SceneManager.sceneCountInBuildSettings; i++) {
            mapIndexList.Add(i);
        }

        //ResetAllRoundInfo();
        roundNumber = 0;

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

        for (int i = 0; i < count; i++) {
            spawnpoints[i] = transform.GetChild(i);
        }
    }

    void Update() {
        if(mapLoadedBoolVariable.Value)
        {
            SpawnAllPlayers();
            mapLoadedBoolVariable.SetValue(false);
        }

        if (roundStarting) {
            StartingCountdown();
            isSummaryShown = false;
        }

        else if (timerIsRunning) {
            RoundTimerCountdown();
        }

        else if (roundEnded) {
            // disable all controls
            EnableAllControls(false);

            if (mapIndexList.Count == 0) {
                // no more maps

                // show end game screen
                SceneManager.LoadScene("Game Over");

                Destroy(this);
            }

            else if (!isSummaryShown) {
                isSummaryShown = true;

                // show the round summary screen
                SceneManager.LoadScene("PostRound");
            }
        }
    }

    private void EnableAllControls(bool enable) {
        foreach (Transform tr in playerConfigManager.transform) {
            if (tr.tag == "Player") {
                PlayerInput playerInput = tr.gameObject.GetComponent<PlayerInput>();

                if (enable) playerInput.ActivateInput();
                else playerInput.DeactivateInput();
            }
        }
    }

    void StartingCountdown() {
        // countdown is ongoing
        if (startingTimer > 0) startingTimer -= Time.deltaTime;

        // countdown is up
        else {
            roundStarting = false;
            timerIsRunning = true;

            // enable controls
            EnableAllControls(true);
        }

        roundStartingCountdownFloatVariable.SetValue(startingTimer);
    }

    void RoundTimerCountdown() {
        if (timeRemaining > 0) timeRemaining -= Time.deltaTime;
        else {
            timerIsRunning = false;

            Debug.Log("Time has run out!");

            // disable all controls
            EnableAllControls(false);

            foreach (Transform tr in playerConfigManager.transform) {
                if (tr.tag == "Player") {
                    PlayerController playerScript = tr.gameObject.GetComponent<PlayerController>();

                    // destroy any held bombs
                    if (playerScript.carriedBomb) Destroy(playerScript.carriedBomb);

                    // hide renderers
                    playerScript.DisableHats();
                    playerScript.EnableModelRenderers(false);
                }
            }

            timeRemaining = 0;
            roundEnded = true;
        }

        TimerFloatVariable.SetValue(timeRemaining);
    }

    // On starting new round, increment round number, reset round timer and change to a new map.
    public void StartNewRound() {
        // based on Scene Index under File -> Build Settings
        // ignore indices of non-playable maps, e.g. Start, Instructions, Lobby, PostRound
        Debug.Log("starting new round");
        int rand = Random.Range(0, mapIndexList.Count);
        int randomMapIndex = mapIndexList[rand];
        mapIndexList.RemoveAt(rand);

        SceneManager.LoadScene(randomMapIndex);

        roundEnded = false;  // resets this from previous round
        roundStarting = true;  // starts the countdown before the round begins

        timeRemaining = gameConstants.roundDuration;
        startingTimer = gameConstants.startingCountdown;

        roundNumber += 1;
        Debug.Log(roundNumber);
    }

    // To be called when returning to main menu or restart pressed.
    public void ResetAllRoundInfo(){
        Awake();
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
        print("spawning");

        int counter = 0;
        Vector3[] vectorList = VectorListVariable.VectorList;

        // set the transform position of all monkeys
        foreach (Transform tr in playerConfigManager.transform) {
            if (tr.tag == "Player") {
                tr.position = vectorList[counter];
                tr.LookAt(Vector3.zero);

                PlayerController playerScript = tr.gameObject.GetComponent<PlayerController>();

                // enable hat and model renderers
                playerScript.EnableHatRenderer(true);
                playerScript.EnableModelRenderers(true);

                // reinitialise player script variables
                playerScript.ReInitVariables();

                counter += 1;
            }
        }
    }
}
