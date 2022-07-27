using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : CommonController {

    private PlayerInput playerInput;
    private Rigidbody playerBody;
    private Vector2 lookDirection;
    private bool isAiming = false;
    public GameConstants gameConstants;

    public PlayerVariable player1Variable;
    public PlayerVariable player2Variable;
    public PlayerVariable player3Variable;
    public PlayerVariable player4Variable;
    public PlayerVariable player5Variable;
    public PlayerVariable player6Variable;
    private PlayerVariable[] playerVarList;
    private PlayerVariable playerVariable;

    // move
    private Vector2 moveVal;

    // spin
    private Vector2 spinVal;
    private Vector3 latestDir;

    // dash
    private bool dashActivated = false;
    private float dashDistance;

    // bomb
    public Transform bombContainer;

    private Rigidbody bombBody;
    private ExplosiveController bombScript;
    private GameObject pickableBomb = null;
    private GameObject carriedBomb = null;
    private int bombThrowForce;


    private void Start() {
        playerInput = GetComponent<PlayerInput>();
        playerBody = GetComponent<Rigidbody>();
        
        playerVarList = new PlayerVariable[] {player1Variable, player2Variable, player3Variable, player4Variable, player5Variable, player6Variable};
        print(playerInput.user.id);
        playerVariable = playerVarList[playerInput.user.id];
        // playerVariable = player1Variable;

        playerVariable.SetMoveSpeed(gameConstants.playerMoveSpeed);
        dashDistance = gameConstants.dashDistance;
        bombThrowForce = gameConstants.bombThrowForce;
    }

    // automatic callback when corresponding input is detected
    private void OnMove(InputValue value) {
        if (!isAiming) {
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
    private void OnPickUpDrop() {
        // check that not carrying any bombs, and a bomb is pickable
        if (!carriedBomb && pickableBomb) {
            bombScript = pickableBomb.GetComponent<ExplosiveController>().getScript();
            bombScript.AttachToPlayer(gameObject);
            pickableBomb = null;
        }

        // if already carrying a bomb, drop it
        else if (carriedBomb) {
            // carriedBomb will be set to null as bomb.DetachFromPlayer() calls SetCarryNull()
            DropBomb();
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

            // detach bomb from player
            bombScript.DetachFromPlayer();

            // set bomb inAir to true
            bombScript.setInAir(true);

            // activate bomb
            bombScript.ActivateBomb();

            // normalize direction vector and throw bomb
            latestDir.y = 0.1f;
            bombBody.AddForce(latestDir / latestDir.magnitude * bombThrowForce, ForceMode.Impulse);
        }
    }

    public void SetCarryBomb(GameObject bombObject) {
        carriedBomb = bombObject;
    }

    public void DropBomb() {
        if (carriedBomb) {
            bombScript.DetachFromPlayer();
        }
    }

    private void Update() {
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

    private void FixedUpdate() {
        // move
        Vector3 movementTranslation = new Vector3(moveVal.x, 0, moveVal.y);
        playerBody.AddForce(movementTranslation * playerVariable.MoveSpeed, ForceMode.Impulse);

        // dash
        if (dashActivated) {
            dashActivated = false;

            Vector3 dashForce;

            // if character is moving, apply dash in direction of movement
            if (movementTranslation != Vector3.zero) dashForce = movementTranslation;

            // if character is stationary, apply dash in direction that player is facing
            else dashForce = latestDir;

            playerBody.AddForce(dashForce / dashForce.magnitude * dashDistance, ForceMode.Impulse);
        }
    }

    // use OnCollisionEnter to check bomb in-air hit
    private void OnCollisionEnter(Collision col) {
        if (col.gameObject.CompareTag("Bomb")) {
            System.String colliderName = col.GetContact(0).thisCollider.name;
            ExplosiveController colBombScript = col.gameObject.GetComponent<ExplosiveController>();

            if (colliderName == "FrontCollider" && colBombScript.getInAir() && !carriedBomb) {
                // can only pick up if bomb comes from the front and empty hands
                colBombScript.AttachToPlayer(gameObject);
            }

            else if (colliderName == "SideBackCollider" && colBombScript.getInAir() && colBombScript.getActive()) {
                // activate bomb effect immediately if hit side or back
                StartCoroutine(colBombScript.ExplodeNow());
            }
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.gameObject.CompareTag("Bomb") || other.gameObject.CompareTag("Rock")) {
            pickableBomb = other.gameObject;
        }

        else if (other.gameObject.CompareTag("OutOfBounds")) {
            KillPlayer();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Bomb") || other.gameObject.CompareTag("Rock")) {
            pickableBomb = null;
        }
    }

    public void KillPlayer() {
        Debug.Log("Player dead");

        // disable controls
        playerInput.DeactivateInput();

        // drop any carried bombs
        // no need to light the fuse because it will be handled from within bomb script
        if (carriedBomb) bombScript.DetachFromPlayer();


        // play death animation

        // wait for animation to finish playing


        // setting player object to inactive makes a new one spawn when input is detected
        // so just render the player invisible and uncollidable
        EnableAllColliders(false);
        EnableAllRenderers(false);
    }
}
