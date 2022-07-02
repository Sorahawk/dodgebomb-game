using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour {
    private float moveSpeed = 10;
    private Vector2 moveVal;

    private Vector2 spinVal;
    private Vector3 latestDir;

    void OnMove(InputValue value) {
        moveVal = value.Get<Vector2>();
    }

    void OnSpin(InputValue value) {
        spinVal = value.Get<Vector2>();
    }

    void Update() {
        // apply translation to the world axes, so movement direction is constant and follows thumbsticks
        Vector3 translation = new Vector3(moveVal.x, 0, moveVal.y) * moveSpeed * Time.deltaTime;
        transform.Translate(translation, Space.World);

        // rotate object corresponding to spin input, by looking at world coordinates
        float lookX = transform.position.x + spinVal.x;
        float lookZ = transform.position.z + spinVal.y;
        transform.LookAt(new Vector3(lookX, 0, lookZ));
    }
}
