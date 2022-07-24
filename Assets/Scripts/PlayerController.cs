using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : CommonController {

    private PlayerInput playerInput;
    private Rigidbody playerBody;
    private Vector2 lookDirection;
    private bool isAiming = false;

    // move
    private float moveSpeed = 5;
    private Vector2 moveVal;

    // spin
    private Vector2 spinVal;
    private Vector3 latestDir;

    // dash
    private bool dashActivated = false;
    private float dashDistance = 70;

    // bomb
    public Transform bombContainer;

    private Rigidbody bombBody;
    private ExplosiveController bombScript;
    private GameObject pickableBomb = null;
    private GameObject carriedBomb = null;
    private int bombThrowForce = 30;


    private void Start() {
        playerInput = GetComponent<PlayerInput>();
        playerBody = GetComponent<Rigidbody>();
    }

    // automatic callback when corresponding input is detected
    private void OnMove(InputValue value) {
        if (!isAiming){
            moveVal = value.Get<Vector2>();
        }
    }

    // automatic callback when corresponding input is detected
    private void OnSpin(InputValue value) {
        spinVal = value.Get<Vector2>();
    }

    // automatic callback when corresponding input is detected
    private void OnDash() {
        if (!isAiming && !carriedBomb){
            dashActivated = true;
        }
    }

    // automatic callback when corresponding input is detected
    private void OnThrow(){
        // return if no bomb carried
        if (!carriedBomb) {
            isAiming = false;
            return;
        }

        // this function is only called if the appropriate key is pressed or released
        // if IsPressed() is true, button was pressed; if false, button was released
        bool isPress = playerInput.actions["Throw"].IsPressed();

        // button pressed; aiming bomb
        if (isPress){
            moveVal = Vector2.zero;
            isAiming = true;
        }

        // button released; throw bomb
        // need to check that isAiming is true
        // if players press and hold Throw before picking up bomb, it will be thrown straight away when released
        else if (!isPress && isAiming) {
            isAiming = false;

            // declare bombBody here before carriedBomb is nullified by DetachFromPlayer()
            bombBody = carriedBomb.GetComponent<Rigidbody>();
            bombScript = carriedBomb.GetComponent<ExplosiveController>();

            // detach bomb from player
            bombScript.DetachFromPlayer();

            // set bomb inAir to true
            bombScript.setInAir(true);

            // activate bomb
            bombScript.ActivateBomb();

            // normalize direction vector and throw bomb
            bombBody.AddForce(latestDir / latestDir.magnitude * bombThrowForce, ForceMode.Impulse);
        }
    }

    // automatic callback when corresponding input is detected
    private void OnPickUpDrop() {
        // check that not carrying any bombs, and a bomb is pickable
        if (!carriedBomb && pickableBomb) {
            pickableBomb.GetComponent<ExplosiveController>().AttachToPlayer(gameObject);
            carriedBomb = pickableBomb;
            pickableBomb = null;
        }

        // if already carrying a bomb, drop it
        else if (carriedBomb) {
            // carriedBomb will be set to null as bomb.DetachFromPlayer() calls SetCarryNull()
            carriedBomb.GetComponent<ExplosiveController>().DetachFromPlayer();
        }
    }

    public void SetCarryNull() {
        carriedBomb = null;
    }

    private void Update() {
        // move
        Vector3 movementTranslation = new Vector3(moveVal.x, 0, moveVal.y);
        playerBody.AddForce(movementTranslation * moveSpeed, ForceMode.Impulse);

        // dash
        if (dashActivated) {
            dashActivated = false;

            // if character is moving, apply dash in direction of movement
            if (movementTranslation != Vector3.zero) {
                playerBody.AddForce(movementTranslation / movementTranslation.magnitude * dashDistance, ForceMode.Impulse);
            }

            // if character is stationary, apply dash in direction that player is facing
            else playerBody.AddForce(latestDir / latestDir.magnitude * dashDistance, ForceMode.Impulse);
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
    private void OnCollisionStay(Collision col) {
        if (col.gameObject.CompareTag("Bomb")) {
            System.String colliderName = col.GetContact(0).thisCollider.name;
            bombScript = col.gameObject.GetComponent<ExplosiveController>();

            // can only pick up if bomb comes from the front and empty hands
            if (colliderName == "FrontCollider") {
                if (!carriedBomb && bombScript.getInAir()) {
                    col.gameObject.GetComponent<ExplosiveController>().AttachToPlayer(gameObject);
                } else pickableBomb = col.gameObject;
            }

            // activate bomb effect immediately if hit side or back
            else if (colliderName == "SideBackCollider") {
                if (bombScript.getActive() && bombScript.getInAir()) {
                    StartCoroutine(bombScript.StartFuse());
                }
            }
        }
    }

    private void OnCollisionExit(Collision col) {
        if (col.gameObject.CompareTag("Bomb")) {
            pickableBomb = null;
        }
    }

    public void KillPlayer() {
        // disable controls
        playerInput.DeactivateInput();

        // drop any carried bombs
        // no need to light the fuse because it will be handled from within bomb script
        if (carriedBomb) carriedBomb.GetComponent<ExplosiveController>().DetachFromPlayer();


        // play death animation

        // wait for animation to finish playing


        // setting player object to inactive makes a new one spawn when input is detected
        // so just render the player invisible and uncollidable
        EnableAllColliders(false);
        EnableAllRenderers(false);
    }
}
