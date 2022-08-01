using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject pauseMenu; 
    public GameObject homeScreen;
    public GameObject restartButton, resumeButton, QuitButton;

    // public PlayerInput playerInput;

    // Update is called once per frame
    void Update()
    {
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
}
