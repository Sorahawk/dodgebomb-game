using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class PauseMenu : MonoBehaviour {
    public GameObject pauseMenu; 
    public GameObject HUD;

    public BoolVariable pausedBoolVariable;
    public BoolVariable QuitorRestartBooleanVariable;

    void Update() {
        pauseMenu.SetActive(pausedBoolVariable.Value);
        HUD.SetActive(!pausedBoolVariable.Value);

        if (pausedBoolVariable.Value) Time.timeScale = 0;
        else Time.timeScale = 1;
    }

    public void RestartButtonPressed(){
        Debug.Log("restart pressed");

        Time.timeScale = 1;

        pausedBoolVariable.SetValue(false);
        QuitorRestartBooleanVariable.SetValue(true);

        SceneManager.LoadScene("Lobby");
    }

    public void ResumeButtonPressed(){
        Debug.Log("resume pressed");

        Time.timeScale = 1;

        pausedBoolVariable.SetValue(false);

        pauseMenu.SetActive(false);
    }

    public void QuitButtonPressed(){
        Debug.Log("quit pressed");

        Time.timeScale = 1;

        pausedBoolVariable.SetValue(false);
        QuitorRestartBooleanVariable.SetValue(true);

        SceneManager.LoadScene("Start");    
    }
}
