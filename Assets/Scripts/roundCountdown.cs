using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class roundCountdown : MonoBehaviour {
    public GameObject roundStartingText;
    public GameObject countdown;
    public GameObject background;
    public FloatVariable roundStartingCountdownFloatVariable;

    void Update() {
        if (roundStartingCountdownFloatVariable.Value <= 0) {
            roundStartingText.SetActive(false);
            countdown.SetActive(false);
            background.SetActive(false);
        }

        else {
            roundStartingText.SetActive(true);
            countdown.SetActive(true);
            background.SetActive(true);
            countdown.GetComponent<Text>().text = "" + Mathf.FloorToInt(roundStartingCountdownFloatVariable.Value);
        }
    }
}
