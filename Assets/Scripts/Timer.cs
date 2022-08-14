using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public FloatVariable timeRemaining;
    public Text timerText;

    // Update is called once per frame
    void Update()
    {
        DisplayTime(timeRemaining.Value);
    }

    public void DisplayTime(float timeToDisplay){
        if (timeToDisplay <0)
        {
            timeToDisplay = 0;
        }
        float minutes = Mathf.FloorToInt(timeToDisplay/60);
        float seconds = Mathf.FloorToInt(timeToDisplay%60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
