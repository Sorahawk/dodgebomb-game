using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : CommonController {

    public PlayerInput playerInput;
    private Rigidbody playerBody;
    private Animator playerAnimator;
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

    private float playerCurrentSpeed;
    private bool inSand = false;


    private void Start() {
        playerInput = GetComponent<PlayerInput>();
        playerBody = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
        
        playerVarList = new PlayerVariable[] {player1Variable, player2Variable, player3Variable, player4Variable, player5Variable, player6Variable};

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
            bombScript.AttachToPlayer(gameObject, playerInput.playerIndex);
            pickableBomb = null;
        }

        // if already carrying a bomb, drop it
        else if (carriedBomb) {
            // carriedBomb will be set to null as bomb.DetachFromPlayer() calls SetCarryNull()
            DropBomb();
        }
    }

    // automatic callback when corresponding input is detected
    private void OnThrow() {
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

            // play throw animation
            playerAnimator.SetTrigger("bombThrow");
            playerAnimator.SetBool("holdingBomb", false);
        }
    }

    // automatic callback when corresponding input is detected
    private void OnUsePowerup() {
        print(playerVariable.Powerup);
        if (playerVariable.Powerup==1) {
            // confusion (call a script that input current player index)
            print("confusion");
            StartCoroutine(Confuse((int)playerInput.user.id));
        } else if (playerVariable.Powerup==2) {
            // Power throw
            print("power throw");
        } else if (playerVariable.Powerup==3) {
            // Shield
            print("shield");
        } else if (playerVariable.Powerup==4) {
            // Speed
            print("speed");
            StartCoroutine(SpeedPowerup());
        } else if (playerVariable.Powerup==5) {
            // Trap
            // spawn trap prefab at current player position
            print("trap");
        }
        playerVariable.SetPowerup(0);
    }

    public void SetCarryBomb(GameObject bombObject) {
        carriedBomb = bombObject;

        // if controller is keyboard and mouse, Gamepad.current is null
        if (carriedBomb == null && Gamepad.current != null) Gamepad.current.SetMotorSpeeds(0, 0);
        else {
            bombScript = pickableBomb.GetComponent<ExplosiveController>().getScript();
            playerAnimator.SetBool("holdingBomb", true);

            if (bombScript.getActive() && Gamepad.current != null) {
                // this line alone is enough to set the rumble speeds, no need declare anything above
                Gamepad.current.SetMotorSpeeds(0, 0.5f);
            }
        }
    }

    public void DropBomb() {
        if (carriedBomb) {
            bombScript.DetachFromPlayer();
            playerAnimator.SetBool("holdingBomb", false);
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
        if (inSand){
            playerCurrentSpeed = gameConstants.playerMoveSpeed/2;
        }else if (!inSand){
            playerCurrentSpeed = gameConstants.playerMoveSpeed;
        }
        playerVariable.SetMoveSpeed(playerCurrentSpeed);
        

        // move
        Vector3 movementTranslation = new Vector3(moveVal.x, 0, moveVal.y);

        if (movementTranslation == Vector3.zero) playerAnimator.SetBool("isRunning", false);
        else playerAnimator.SetBool("isRunning", true);

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
                colBombScript.AttachToPlayer(gameObject, playerInput.playerIndex);
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
        else if (other.gameObject.CompareTag("Quicksand")) {
            // playerVariable.SetMoveSpeed(gameConstants.playerMoveSpeed/2);
            inSand = true;
        }
        else if (other.gameObject.CompareTag("Powerup")) {
            playerVariable.SetPowerup(other.gameObject.GetComponent<Powerup>().powerup_id);
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Bomb") || other.gameObject.CompareTag("Rock")) {
            pickableBomb = null;
        }
        else if (other.gameObject.CompareTag("Quicksand")) {
            // playerVariable.SetMoveSpeed(gameConstants.playerMoveSpeed);
            inSand= false;
        }
    }

    public void StunPlayer() {
        // disable controls
        playerInput.DeactivateInput();

        // play stun animation
        playerAnimator.SetTrigger("stunTrigger");

        StartCoroutine(StunDelay());
    }

    private IEnumerator StunDelay() {
        yield return new WaitForSeconds(2);

        // re-enable controls
        playerInput.ActivateInput();
    }

    //Speed Boost Powerup
    private IEnumerator SpeedPowerup() {
        playerVariable.SetMoveSpeed(playerVariable.MoveSpeed*2);
        yield return new WaitForSeconds(5);
        playerVariable.SetMoveSpeed(playerVariable.MoveSpeed/2);
    }

    // Confusion Powerup
    private IEnumerator Confuse(int playerIndex) {
        int i = 0;
        foreach (PlayerVariable var in playerVarList) {
            if (i!=playerIndex) {
                var.SetMoveSpeed(var.MoveSpeed*-1);
            }
            i++;
        }
        yield return new WaitForSeconds(5);
        i = 0;
        foreach (PlayerVariable var in playerVarList) {
            if (i!=playerIndex) {
                var.SetMoveSpeed(var.MoveSpeed*-1);
            }
            i++;
        }
    }

    public void KillPlayer() {
        Debug.Log("Player dead");

        // disable controls
        playerInput.DeactivateInput();

        // drop any carried bombs
        // no need to light the fuse because it will be handled from within bomb script
        if (carriedBomb) bombScript.DetachFromPlayer();

        // turn off hat renderers
        DisableHats();

        // play death animation
        playerAnimator.SetTrigger("deathTrigger");

        // wait for animation to finish playing
        StartCoroutine(DeathDisappear());
    }

    private void DisableHats() {
        Renderer[] hats = transform.Find("Hats").GetComponentsInChildren<Renderer>();

        foreach (Renderer hat in hats) {
            hat.enabled = false;
        }
    }

    private IEnumerator DeathDisappear() {
        yield return new WaitForSeconds(1.5f);

        // setting player object to inactive makes a new one spawn when input is detected
        // so just render the player invisible and uncollidable
        EnableAllColliders(false);
        EnableAllRenderers(false);
    }
}
