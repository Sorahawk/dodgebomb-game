using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class MenuController : MonoBehaviour {

    public GameObject playButton;
    public GameObject backButton;
    public GameObject Canvas;

    public void LoadScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadCanvas() {
        Canvas.SetActive(true);
    }
}
