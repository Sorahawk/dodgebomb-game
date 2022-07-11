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
    private float dashDistance = 100;

    // bomb
    private GameObject pickableBomb = null;
    private GameObject carriedBomb = null;
    private int bombThrowForce = 30;

    private Rigidbody playerBody;
    private Vector2 faceDirection;

    void Start(){
        playerBody = GetComponent<Rigidbody>();
    }
    void OnMove(InputValue value) {
        moveVal = value.Get<Vector2>();
        if(moveVal.x != 0 || moveVal.y != 0){
            faceDirection = moveVal;
        }
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
            // TODO: debug why bomb becomes so 'heavy' after dropping it

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

            // activate bomb fuse
            StartCoroutine(carriedBomb.GetComponent<BombController>().StartFuse());

            // throw bomb in direction player is facing
            carriedBomb.GetComponent<Rigidbody>().AddForce(latestDir * bombThrowForce, ForceMode.Impulse);

            carriedBomb = null;
        }
    }

    void Update() {
        // move
        // apply translation to the world axes, so movement direction is constant and follows thumbsticks
        Vector3 translation = new Vector3(moveVal.x/2, 0, moveVal.y/2);

        Vector3 movementTranslation = new Vector3(moveVal.x, 0, moveVal.y);
        transform.Translate(movementTranslation * moveSpeed * Time.deltaTime, Space.World);

        // playerBody.AddForce(translation,ForceMode.Impulse);
        // playerBody.velocity = new Vector3(moveVal.x *10, 0, moveVal.y*10);
        // playerBody.velocity = translation;

        // dash
        if (dashActivated) {
            dashActivated = false;

            // if character is moving, apply dash in direction of movement
            // otherwise if character is stationary, apply dash in direction that player is facing

            if (translation != Vector3.zero) {
                // transform.Translate(translation * dashDistance, Space.World);
                // Vector3 movementDirection = new Vector3(translation.x*2,0,translation.y*2);
                playerBody.AddForce(translation * dashDistance,ForceMode.Impulse);
            } else {
                // transform.Translate(Vector3.forward * dashDistance);
                Vector3 stationaryDirection = new Vector3(faceDirection.x/(3/2),0,faceDirection.y/(3/2));
                playerBody.AddForce(stationaryDirection * dashDistance,ForceMode.Impulse);
            }
        }

        // spin
        // rotate character according to spin input, by "looking at" corresponding world coordinates
        float lookX = transform.position.x + spinVal.x;
        float lookY = transform.position.y; // character has to look at its own "height"
        float lookZ = transform.position.z + spinVal.y;

        transform.LookAt(new Vector3(lookX, lookY, lookZ));

        if (spinVal != Vector2.zero) {
            latestDir = new Vector3(spinVal.x, 0, spinVal.y);
        }
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