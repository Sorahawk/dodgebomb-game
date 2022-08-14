using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class MenuController : MonoBehaviour {

    public GameObject playButton;
    public GameObject backButton;
    public BoolVariable QuitorRestartBooleanVariable;

    public void LoadScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadCanvas(GameObject canvas) {
        canvas.SetActive(true);
    }

    public void DisableCanvas(GameObject canvas) {
        canvas.SetActive(false);
    }

    public void BackButtonPressed(){
        QuitorRestartBooleanVariable.SetValue(true);
        SceneManager.LoadScene("Start");
    }
}



