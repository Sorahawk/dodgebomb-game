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

    private Camera mainCamera;

    void Start() {
        mainCamera = transform.Find("Pause Canvas").GetComponent<Canvas>().worldCamera;
    }

    void Update() {
        bool isPaused = pausedBoolVariable.Value;

        pauseMenu.SetActive(isPaused);
        HUD.SetActive(!isPaused);

        // turn audio on or off
        ActivateAudio(!isPaused);

        if (isPaused) Time.timeScale = 0;
        else Time.timeScale = 1;
    }

    private void ActivateAudio(bool activate) {
        AudioSource cameraAudio = mainCamera.GetComponent<AudioSource>();
        cameraAudio.enabled = activate;
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
