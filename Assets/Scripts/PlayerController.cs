using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour {
    private float moveSpeed = 10;
    private Vector2 moveVal;

    private Vector2 spinVal;
    private Vector3 latestDir;

    private bool dashActivated = false;
    private float dashDistance = 2;

    void OnMove(InputValue value) {
        moveVal = value.Get<Vector2>();
    }

    void OnSpin(InputValue value) {
        spinVal = value.Get<Vector2>();
    }

    void OnDash() {
        dashActivated = true;
    }

    void Update() {
        // move
        // apply translation to the world axes, so movement direction is constant and follows thumbsticks
        Vector3 translation = new Vector3(moveVal.x, 0, moveVal.y);
        transform.Translate(translation * moveSpeed * Time.deltaTime, Space.World);

        // dash
        if (dashActivated) {
            dashActivated = false;

            // if character is moving, apply dash in direction of movement
            // otherwise if character is stationary, apply dash in direction that player is facing

            if (translation != Vector3.zero) {
                transform.Translate(translation * dashDistance, Space.World);
            } else {
                transform.Translate(Vector3.forward * dashDistance);
            }
        }

        // spin
        // rotate character according to spin input, by "looking at" corresponding world coordinates
        float lookX = transform.position.x + spinVal.x;
        float lookY = transform.position.y; // character has to look at its own "height"
        float lookZ = transform.position.z + spinVal.y;

        transform.LookAt(new Vector3(lookX, lookY, lookZ));
    }
}
