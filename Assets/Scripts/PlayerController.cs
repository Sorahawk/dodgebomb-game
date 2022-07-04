using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour {
    // move
    private float moveSpeed = 10;
    private Vector2 moveVal;

    // spin
    private Vector2 spinVal;
    private Vector3 latestDir;

    // dash
    private bool dashActivated = false;
    private float dashDistance = 2;

    // bomb
    public GameObject pickableBomb = null;
    public GameObject carriedBomb = null;

    void OnMove(InputValue value) {
        moveVal = value.Get<Vector2>();
    }

    void OnSpin(InputValue value) {
        spinVal = value.Get<Vector2>();
    }

    void OnDash() {
        dashActivated = true;
    }

    void OnPickUpDrop() {
        // check that not carrying any bombs, and a bomb is pickable
        if (!carriedBomb && pickableBomb) {
            // attach bomb to player
            pickableBomb.transform.SetParent(gameObject.transform);
            carriedBomb = pickableBomb;
            pickableBomb = null;

            // turn off bomb's useGravity so it stays with the player
            carriedBomb.GetComponent<Rigidbody>().useGravity = false;
        }

        // if already carrying a bomb, drop it
        else if (carriedBomb) {
            // turn the bomb's useGravity back on
            carriedBomb.GetComponent<Rigidbody>().useGravity = true;

            // detach bomb
            carriedBomb.transform.SetParent(null);
            carriedBomb = null;
        }
    }

    void OnThrow() {
        // check if a bomb is carried
        if (carriedBomb) {
            // turn on bomb's useGravity
            carriedBomb.GetComponent<Rigidbody>().useGravity = true;

            // detach bomb from player
            carriedBomb.transform.SetParent(null);
            carriedBomb = null;

            // TODO: throw bomb
        }
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

    // use OnCollisionStay to reconfirm object collision every frame
    void OnCollisionStay(Collision col) {
        if (col.gameObject.CompareTag("Bomb")) {
            System.String colliderName = col.GetContact(0).thisCollider.name;

            // can only pick up if bomb is from the front
            if (colliderName == "FrontCollider") {
                pickableBomb = col.gameObject;

                // TODO: check whether bomb is in-air or on the ground
                // if in air, then pick up automatically if not already carrying a bomb
            }
        }
    }

    void OnCollisionExit(Collision col) {
        if (col.gameObject.CompareTag("Bomb")) {
            pickableBomb = null;
        }
    }
}