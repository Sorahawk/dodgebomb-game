using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Audio;


public class PlayerController : CommonController {

    public PlayerInput playerInput;
    private Rigidbody playerBody;
    private Animator playerAnimator;
    private Vector2 lookDirection;
    private bool isAiming = false;
    private bool isDead = false;

    public GameConstants gameConstants;
    public PlayerVariable player1Variable;
    public PlayerVariable player2Variable;
    public PlayerVariable player3Variable;
    public PlayerVariable player4Variable;
    public PlayerVariable player5Variable;
    public PlayerVariable player6Variable;
    private PlayerVariable[] playerVarList;
    private PlayerVariable playerVariable;
    private RoundManager roundManager;
    
    public BoolVariable pausedBoolVariable;
    public GameObject bearTrap;

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
    public GameObject carriedBomb = null;
    private int bombThrowForce;
    private int powerThrowForce;

    private float playerCurrentSpeed;
    private bool isDash = true;
    private bool isShield = false;

    // environmental
    private bool inSand = false;
    private bool onIce = false;
    private float originalDrag;
    private bool respawnInvulnerable  = false;
    private bool onFire = false;

    // hat
    private int hatIndex = -1;
    private Renderer[] hatArray;

    private int playerIndex;
    private HUDManager hudManager;

    protected AudioSource footstepsAudioSource;
    protected AudioSource throwingAudioSource;
    protected AudioSource dashAudioSource;
    public AudioClip footstepsAudioClip;
    public AudioClip throwingAudioClip;
    public AudioClip dashAudioClip;
    public AudioMixerGroup footstepsMixer;
    public AudioMixerGroup throwingMixer;
    public AudioMixerGroup dashMixer;

    public void setHUDManager(HUDManager HUDManager) {
        hudManager = HUDManager;
    }

    public void ReInitVariables() {
        isAiming = false;
        dashActivated = false;
        isDash = true;
        isShield = false;
        inSand = false;
        onIce = false;
        onFire = false;
        playerBody.drag = originalDrag;
    }

    private void Start() {
        playerInput = GetComponent<PlayerInput>();
        playerBody = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();

        playerIndex = playerInput.playerIndex;
        
        playerVarList = new PlayerVariable[] {player1Variable, player2Variable, player3Variable, player4Variable, player5Variable, player6Variable};
        playerVariable = playerVarList[playerIndex];

        playerVariable.SetMoveSpeed(gameConstants.playerMoveSpeed);
        dashDistance = gameConstants.dashDistance;
        bombThrowForce = gameConstants.bombThrowForce;
        powerThrowForce = gameConstants.powerThrowForce;

        hatArray = transform.Find("Hats").GetComponentsInChildren<Renderer>();
        roundManager = transform.parent.gameObject.transform.GetChild(0).gameObject.GetComponent<RoundManager>();

        originalDrag = playerBody.drag;

        // add audiosource component via scripting
        footstepsAudioSource = gameObject.AddComponent<AudioSource>();
        footstepsAudioSource.clip = footstepsAudioClip;
        footstepsAudioSource.outputAudioMixerGroup = footstepsMixer;

        throwingAudioSource = gameObject.AddComponent<AudioSource>();
        throwingAudioSource.clip = throwingAudioClip;
        throwingAudioSource.outputAudioMixerGroup = throwingMixer;

        dashAudioSource = gameObject.AddComponent<AudioSource>();
        dashAudioSource.clip = dashAudioClip;
        dashAudioSource.outputAudioMixerGroup = dashMixer;
    }

    private void OnPause() {
        pausedBoolVariable.SetValue(!pausedBoolVariable.Value);
    }

    private void OnMove(InputValue value) {
        if (!isAiming) {
            moveVal = value.Get<Vector2>();
        }
    }

    private void OnSpin(InputValue value) {
        spinVal = value.Get<Vector2>();
    }

    private void OnDash() {
        if (!isAiming && !carriedBomb && isDash){
            dashActivated = true;
            isDash = false;
            //play audio when dashing
            dashAudioSource.Play();
            StartCoroutine(DashReset());
            StartCoroutine(hudManager.ActivateDashCooldown(playerIndex));
        }
    }

    private void OnPickUpDrop() {
        // check that not carrying any bombs, and a bomb is pickable
        if (!carriedBomb && pickableBomb) {
            bombScript = pickableBomb.GetComponent<ExplosiveController>();
            bombScript.AttachToPlayer(gameObject, playerIndex);
            pickableBomb = null;
        }

        // if already carrying a bomb, drop it
        else if (carriedBomb) {
            // carriedBomb will be set to null as bomb.DetachFromPlayer() calls SetCarryNull()
            DropBomb();
        }
    }

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

            //play audio when throwing
            throwingAudioSource.Play();

            // declare bombBody here before carriedBomb is nullified by DetachFromPlayer()
            bombBody = carriedBomb.GetComponent<Rigidbody>();

            // detach bomb from player
            bombScript.DetachFromPlayer();

            // set bomb inAir to true
            bombScript.setInAir(true);

            // activate bomb
            bombScript.ActivateBomb();

            latestDir.y = 0.1f;
            // normalize direction vector and throw bomb
            bombBody.AddForce(latestDir / latestDir.magnitude * bombThrowForce, ForceMode.Impulse);

            // play throw animation
            playerAnimator.SetTrigger("bombThrow");
            playerAnimator.SetBool("holdingBomb", false);
        }
    }

    private void OnUsePowerup() {
        print(playerVariable.Powerup);

        if (playerVariable.Powerup == 1) {
            // confusion (call a script that input current player index)
            print("confusion");
            StartCoroutine(Confuse(playerIndex));
        } else if (playerVariable.Powerup == 2) {
            // Shield
            print("shield");
            isShield = true;

            // enable shield VFX
            transform.Find("Shield").gameObject.SetActive(true);
        } else if (playerVariable.Powerup == 3) {
            // Speed
            print("speed");
            StartCoroutine(SpeedPowerup());
        } else if (playerVariable.Powerup == 4) {
            // Trap
            print("trap");
            // instantiate bear trap prefab at current player position
            // tag bear trap to player index
            GameObject trap = Instantiate(bearTrap, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation);
            trap.GetComponent<BearTrapController>().setOwner(playerIndex);
        }

        playerVariable.SetPowerup(0);
        hudManager.HidePowerup(playerIndex);
    }

    public void SetCarryBomb(GameObject bombObject) {
        carriedBomb = bombObject;
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
        if (inSand) playerCurrentSpeed = playerVariable.MoveSpeed / 2;
        else if (onIce) playerCurrentSpeed = playerVariable.MoveSpeed * 2;
        else playerCurrentSpeed = playerVariable.MoveSpeed;

        // move
        Vector3 movementTranslation = new Vector3(moveVal.x, 0, moveVal.y);

        if (movementTranslation == Vector3.zero) playerAnimator.SetBool("isRunning", false);
        else playerAnimator.SetBool("isRunning", true);

        playerBody.AddForce(movementTranslation * playerCurrentSpeed, ForceMode.Impulse);
        
        //play audio when walking
        if (!footstepsAudioSource.isPlaying && movementTranslation != Vector3.zero){
            footstepsAudioSource.Play();
        }

        // dash
        if (dashActivated) {
            dashActivated = false;

            Vector3 dashForce;

            // if character is moving, apply dash in direction of movement
            if (movementTranslation != Vector3.zero) dashForce = movementTranslation;
            
            // if character is stationary, apply dash in direction that player is facing
            else dashForce = latestDir;

            playerBody.AddForce(dashForce / dashForce.magnitude * dashDistance, ForceMode.Impulse);

            // play dash animation
            playerAnimator.SetTrigger("dashTrigger");
        }
    }

    // use OnCollisionEnter to check bomb in-air hit
    private void OnCollisionEnter(Collision col) {
        if (col.gameObject.CompareTag("Bomb")) {
            System.String colliderName = col.GetContact(0).thisCollider.name;
            ExplosiveController colBombScript = col.gameObject.GetComponent<ExplosiveController>();

            if (colliderName == "FrontCollider" && colBombScript.getInAir() && !carriedBomb) {
                print("picking up bomb");

                // can only pick up if bomb comes from the front and empty hands
                colBombScript.AttachToPlayer(gameObject, playerIndex);
            }

            else if (colliderName == "SideBackCollider" && colBombScript.getInAir() && colBombScript.getActive()) {
                // activate bomb effect immediately if hit side or back
                StartCoroutine(colBombScript.ExplodeNow());
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("BearTrap")) {
            if (other.gameObject.GetComponent<BearTrapController>().getOwner() != playerIndex) {
                    if (CheckShield()) DisableShield();
                    else StunPlayer();
            }
        }

        else if (other.gameObject.CompareTag("Fire")) {
            if (onFire) return;

            onFire = true;
            int fireOwnerIndex = other.gameObject.GetComponent<GroundFireController>().getOwner();
            int scoreChange;

            if (CheckShield()) {
                DisableShield();
                onFire = false;
            }

            else {
                if (playerIndex == fireOwnerIndex) scoreChange = 0;
                else scoreChange = 1;

                playerVarList[fireOwnerIndex].ApplyScoreChange(scoreChange);
                KillPlayer();
            }
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.gameObject.CompareTag("Bomb") || other.gameObject.CompareTag("Rock")) {
            pickableBomb = other.gameObject;
        }

        else if (other.gameObject.CompareTag("StickyBomb") && !other.gameObject.GetComponent<ExplosiveController>().getActive()) {
            pickableBomb = other.gameObject;
        }

        else if (other.gameObject.CompareTag("Quicksand")) inSand = true;
        else if (other.gameObject.CompareTag("Ice")) {
            onIce = true;
            playerBody.drag = 3;
        }

        else if (other.gameObject.CompareTag("OutOfBounds")) KillPlayer();

        else if (other.gameObject.CompareTag("Powerup")) {
            int powerupIndex = other.gameObject.GetComponent<Powerup>().powerup_id;
            playerVariable.SetPowerup(powerupIndex);

            // show on HUD
            hudManager.HidePowerup(playerIndex);
            hudManager.ShowPowerup(playerIndex, powerupIndex - 1);

            Destroy(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Bomb") || other.gameObject.CompareTag("StickyBomb") || other.gameObject.CompareTag("Rock")) {
            pickableBomb = null;
        }

        else if (other.gameObject.CompareTag("Quicksand")) inSand = false;
        else if (other.gameObject.CompareTag("Ice")) {
            onIce = false;
            playerBody.drag = originalDrag;
        }
    }

    public void StunPlayer() {
        // disable controls
        playerInput.DeactivateInput();

        // play stun animation
        playerAnimator.SetTrigger("stunTrigger");

        // drop any held bombs
        DropBomb();

        StartCoroutine(StunDelay());
    }

    private IEnumerator StunDelay() {
        yield return new WaitForSeconds(1.5f);

        // re-enable controls
        playerInput.ActivateInput();
    }

    // resetting dash after 3s
    private IEnumerator DashReset() {
        yield return new WaitForSeconds(3);
        isDash = true;
    }

    // speed boost powerup
    private IEnumerator SpeedPowerup() {
        playerVariable.SetMoveSpeed(gameConstants.playerMoveSpeed * 2);

        yield return new WaitForSeconds(5);

        playerVariable.SetMoveSpeed(gameConstants.playerMoveSpeed);
    }

    // confusion powerup
    private IEnumerator Confuse(int playerIndex) {
        invertSpeeds(playerIndex);

        yield return new WaitForSeconds(5);

        invertSpeeds(playerIndex);
    }

    private void invertSpeeds(int playerIndex) {
        int i = 0;

        foreach (PlayerVariable var in playerVarList) {
            if (i != playerIndex) var.SetMoveSpeed(var.MoveSpeed * -1);
            i++;
        }
    }

    // shield powerup
    public bool CheckShield() {
        return isShield;
    }

    public void DisableShield() {
        StartCoroutine(DisablingShield());
    }

    // delayed disable for shield by 0.5s
    private IEnumerator DisablingShield() {
        yield return new WaitForSeconds(0.5f);
        isShield = false;

        // disable shield VFX
        transform.Find("Shield").gameObject.SetActive(false);
    }

    public void DisableHats() {
        int hIndex = 0;
        foreach (Renderer hat in hatArray) {
            if (hat.enabled) {
                hatIndex = hIndex;
                hat.enabled = false;
            }

            hIndex++;
        }
    }

    public void EnableHatRenderer(bool enable) {
        if (hatIndex != -1) hatArray[hatIndex].enabled = enable;
    }

    public void KillPlayer(bool manualRespawn=false) {
        if(!respawnInvulnerable){
            if (!isDead) {
                Debug.Log("Player dead");

                isDead = true;

                // disable controls
                playerInput.DeactivateInput();

                // turn off gravity so doesn't fall through the floor
                playerBody.useGravity = false;

                // disable colliders
                EnableAllColliders(false);

                // drop any carried bombs
                // no need to light the fuse because it will be handled from within bomb script
                DropBomb();

                // turn off hat renderers
                DisableHats();

                // play death animation
                playerAnimator.SetTrigger("deathTrigger");

                // wait for animation to finish playing
                StartCoroutine(DeathDisappear());

                if (!manualRespawn) {
                    StartCoroutine(roundManager.playerDeathRespawn(this.gameObject));
                }
            }
        }

    }

    private IEnumerator DeathDisappear() {
        yield return new WaitForSeconds(2f);

        // setting player object to inactive makes a new one spawn when input is detected
        // so just render the player invisible
        EnableAllRenderers(false);
    }

    public void EnableModelRenderers(bool enable) {
        Renderer[] playerRenderers = transform.Find("Model").GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in playerRenderers) {
            renderer.enabled = enable;
        }
    }

    public void RevivePlayer() {
        Debug.Log("Player respawning");

        // play revive animation
        playerAnimator.SetTrigger("reviveTrigger");

        // turn renderers only for model back on
        EnableModelRenderers(true);

        // shift monkey transform vertically up in the air
        transform.position = new Vector3(transform.position.x, 3, transform.position.z);

        // wait for animation to finish playing before proceeding
        StartCoroutine(ReviveDelay());
        respawnInvulnerable = true;
    }

    private IEnumerator RespawnInvulnerability() {
        yield return new WaitForSeconds(2f);

        // setting player object to inactive makes a new one spawn when input is detected
        // so just render the player invisible
        respawnInvulnerable = false;
    }

    private IEnumerator ReviveDelay() {
        yield return new WaitForSeconds(2f);

        // turn on hat renderer
        EnableHatRenderer(true);

        // enable colliders
        EnableAllColliders(true);

        // turn gravity back on
        playerBody.useGravity = true;

        // enable controls
        playerInput.ActivateInput();

        // set isDead to false
        isDead = false;

        // reinitialise script variables
        ReInitVariables();
        StartCoroutine(RespawnInvulnerability());
    }
}
