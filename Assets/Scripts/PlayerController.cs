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
    private float dashDistance = 150;

    // bomb
    public Transform bombContainer;

    private ExplosiveController bombScript;
    private Rigidbody bombBody;
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

            if(moveVal.x != 0 || moveVal.y != 0){
                lookDirection = moveVal;
            }
        }
    }

    // automatic callback when corresponding input is detected
    private void OnSpin(InputValue value) {
        spinVal = value.Get<Vector2>();
    }

    // automatic callback when corresponding input is detected
    private void OnDash() {
        if (!isAiming){
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
            moveVal = new Vector2(0, 0);
            isAiming = true;
        }

        // button released; throw bomb
        // need to check that isAiming is true
        // if players press and hold Throw before picking up bomb, it will be thrown straight away when released
        else if (!isPress && isAiming) {
            isAiming = false;

            bombBody = carriedBomb.GetComponent<Rigidbody>();
            bombScript = carriedBomb.GetComponent<ExplosiveController>();

            // detach bomb from player
            DetachCarriedBomb();

            // set bomb inAir to true
            bombScript.inAir = true;

            // activate bomb fuse
            StartCoroutine(bombScript.StartFuse());

            // throw bomb in direction player is facing
            bombBody.AddForce(latestDir * bombThrowForce, ForceMode.Impulse);
            carriedBomb = null;
        }
    }

    // automatic callback when corresponding input is detected
    private void OnPickUpDrop() {
        // check that not carrying any bombs, and a bomb is pickable
        if (!carriedBomb && pickableBomb) {
            PickUpBomb(pickableBomb);
        }

        // if already carrying a bomb, drop it
        else if (carriedBomb) {
            DetachCarriedBomb();
        }
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
                playerBody.AddForce(movementTranslation / 2 * dashDistance, ForceMode.Impulse);
            }

            // if character is stationary, apply dash in direction that player is facing
            else {
                Vector3 stationaryDirection = new Vector3(lookDirection.x * 1.5f, 0, lookDirection.y * 1.5f);
                playerBody.AddForce(stationaryDirection * dashDistance, ForceMode.Impulse);
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

    // function to bind the bomb to the character; also used for auto-catching
    private void PickUpBomb(GameObject bombObject) {
        pickableBomb = null;
        carriedBomb = bombObject;

        // attach bomb to player
        carriedBomb.transform.SetParent(bombContainer);
        carriedBomb.transform.localPosition = Vector3.zero;

        // turn on bomb kinematics so position is fixed
        carriedBomb.GetComponent<Rigidbody>().isKinematic = true;
    }

    // function to detach the carried bomb from the player object
    private void DetachCarriedBomb() {
        // turn off bomb kinematics
        carriedBomb.GetComponent<Rigidbody>().isKinematic = false;

        // detach bomb
        carriedBomb.transform.SetParent(null);
        carriedBomb = null;
    }

    // use OnCollisionStay to reconfirm object collision every frame
    private void OnCollisionStay(Collision col) {
        if (col.gameObject.CompareTag("Bomb")) {
            System.String colliderName = col.GetContact(0).thisCollider.name;
            bombScript = col.gameObject.GetComponent<ExplosiveController>();

            // can only pick up if bomb is from the front
            if (colliderName == "FrontCollider") {
                if (bombScript.inAir) {
                    PickUpBomb(col.gameObject);
                } else pickableBomb = col.gameObject;
            }

            else if (colliderName == "SideBackCollider") {
                if (bombScript.inAir) {
                    // explode bomb immediately
                } else pickableBomb = col.gameObject;
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
        if (carriedBomb) DetachCarriedBomb();


        // play death animation

        // wait for animation to finish playing


        // setting player object to inactive makes a new one spawn when input is detected
        // so just render the player invisible and uncollidable
        DisableAllColliders();
        DisableAllRenderers();
    }
}
