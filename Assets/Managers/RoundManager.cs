using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public GameConstants gameConstants;
    
    // timer
    private bool roundStarting = true;
    private float startingTimer = 3;
    private bool timerIsRunning = false;
    private float timeRemaining;

    private Transform[] spawnpoints;
    private int count;

    // Start is called before the first frame update
    void Start()
    {
        timeRemaining = gameConstants.roundDuration;

        // spawn points
        count = transform.childCount;
        spawnpoints = new Transform[count];
        for(int i = 0; i < count; i++){
            spawnpoints[i] = transform.GetChild(i);
        }

        // check how many players and spawn them in the first four spawn points. subsequent respawn

        // disable controls

    }

    // Update is called once per frame
    void Update()
    {

        if (roundStarting)
        {
            // 3 seconds countdown to start round
            StartingCountdown();
        }
        else if (timerIsRunning)
        {
            // 300 seconds round (5 mins)
            RoundTimerCountdown();
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
            }
    }
}
