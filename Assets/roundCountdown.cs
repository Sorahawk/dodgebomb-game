using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class roundCountdown : MonoBehaviour
{
    public GameObject roundStartingText;
    public GameObject countdown;
    public FloatVariable roundStartingCountdownFloatVariable;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(roundStartingCountdownFloatVariable.Value <=0){
            roundStartingText.SetActive(false);
            countdown.SetActive(false);
        }else{
            roundStartingText.SetActive(true);
            countdown.SetActive(true);
            countdown.GetComponent<Text>().text = ""+Mathf.FloorToInt(roundStartingCountdownFloatVariable.Value);
        }

    }
}
