using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public BoolVariable pausedBoolVariable;
    public GameObject pauseMenu; 
    public BoolVariable QuitorRestartBooleanVariable;
    // public GameObject roundManagerObject;
    
    // private RoundManager roundManager;

    // public PlayerInput playerInput;

    void Start(){
        // roundManager = roundManagerObject.GetComponent<RoundManager>();
    }
    // Update is called once per frame
    void Update()
    {
        
        if (pausedBoolVariable.Value){
            pauseMenu.SetActive(pausedBoolVariable.Value);
            Time.timeScale = 0;
        }
        else{
            Time.timeScale = 1;
            pauseMenu.SetActive(pausedBoolVariable.Value);
        }
        
        // playerInput = GetComponent<PlayerInput>();
        // if (playerInput.actions["Pause"].IsPressed())
        // {
        //     PauseUnpause();
        // }
    }

    // public static void PauseUnpause()
    // {
    //     if (!pauseMenu.activeInHierarchy)
    //     {
    //         pauseMenu.SetActive(true);
    //         Time.timeScale = 0f;

    //         EventSystem.current.SetSelectedGameObject(null);
    //         EventSystem.curremt.SetSelectedGameObject(resumeButton);
    //     } else {
    //         pauseMenu.SetActive(false);
    //         Time.timeScale =1f;
            
    //     }
    // }
    public void RestartButtonPressed(){
        Debug.Log("restart pressed");
        // roundManager.ResetAllRoundInfo();
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
        // roundManager.ResetAllRoundInfo();
        Time.timeScale = 1;
        pausedBoolVariable.SetValue(false);
        QuitorRestartBooleanVariable.SetValue(true);
        SceneManager.LoadScene("Start");    
    }
}
