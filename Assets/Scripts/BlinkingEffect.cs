using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkingEffect : MonoBehaviour {

    public Color startColor = Color.green;
    public Color endColor = Color.black;
    public float speed = -1;
    [Range(0,10)]
    Renderer ren;

    void Awake() {
        ren = GetComponent<Renderer>();
    }

    void Update() {
        ren.material.color = Color.Lerp(startColor, endColor, Mathf.PingPong(Time.time * speed, 1));
    }
}
