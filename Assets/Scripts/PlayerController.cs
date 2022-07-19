using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour {
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
    private GameObject pickableBomb = null;
    private GameObject carriedBomb = null;
    private int bombThrowForce = 30;

    private Rigidbody playerBody;
    private Vector2 faceDirection;
    private DeviceInput playerControls;
    private bool currentlyAiming = false;

    //bomb pickup logic
    public Transform bombContainer;

    void Start(){
        playerBody = GetComponent<Rigidbody>();
        // playerControls = new DeviceInput();

    }

    void Awake(){
        playerControls = new DeviceInput();
        playerControls.Default.Throw.performed += OnThrowPerformed;
        playerControls.Default.Throw.canceled += OnThrowCanceled;
    }

    void OnEnable(){
        playerControls.Enable();
    }

    void OnDisable(){
        playerControls.Disable();
    }

    void OnMove(InputValue value) {
        if (!currentlyAiming){
            moveVal = value.Get<Vector2>();

            if(moveVal.x != 0 || moveVal.y != 0){
                faceDirection = moveVal;
            }
        }

    }

    void OnSpin(InputValue value) {
        spinVal = value.Get<Vector2>();
    }

    void OnDash() {
        if (!currentlyAiming){
            dashActivated = true;
        }
        
    }
    void OnThrowPerformed(InputAction.CallbackContext context){
        if (carriedBomb){
            moveVal = new Vector2(0,0);
            currentlyAiming = true;
        }
    }

    void OnThrowCanceled(InputAction.CallbackContext context){
        currentlyAiming = false;

        // check if a bomb is carried
        if (carriedBomb) {
            // turn on bomb's useGravity
            // carriedBomb.GetComponent<Rigidbody>().useGravity = true;
            carriedBomb.GetComponent<Rigidbody>().isKinematic = false;

            // detach bomb from player
            carriedBomb.transform.SetParent(null);
            carriedBomb.transform.localScale = Vector3.one;

            // set bomb inAir to true
            carriedBomb.GetComponent<BombController>().inAir = true;

            // activate bomb fuse
            StartCoroutine(carriedBomb.GetComponent<BombController>().StartFuse());

            // throw bomb in direction player is facing
            carriedBomb.GetComponent<Rigidbody>().AddForce(latestDir * bombThrowForce, ForceMode.Impulse);
            carriedBomb = null;
        }
    }

    void OnPickUpDrop() {
        // check that not carrying any bombs, and a bomb is pickable
        if (!carriedBomb && pickableBomb) {
            PickUpBomb(pickableBomb);
        }

        // if already carrying a bomb, drop it
        else if (carriedBomb) {
            carriedBomb.GetComponent<Rigidbody>().isKinematic = false;
            // turn the bomb's useGravity back on
            //carriedBomb.GetComponent<Rigidbody>().useGravity = true;

            // detach bomb
            carriedBomb.transform.SetParent(null);
            carriedBomb = null;
        }
    }

    void PickUpBomb(GameObject bombObject) {
        pickableBomb = null;

        // attach bomb to player
        // pickableBomb.transform.SetParent(gameObject.transform);
        carriedBomb = bombObject;

        carriedBomb.transform.SetParent(bombContainer);
        carriedBomb.transform.localPosition = Vector3.zero;
        // carriedBomb.transform.localRotation = Quaternion.Euler(Vector3.zero);
        // carriedBomb.transform.localScale = Vector3.one;

        carriedBomb.GetComponent<Rigidbody>().isKinematic = true;
        // turn off bomb's useGravity so it stays with the player
        // carriedBomb.GetComponent<Rigidbody>().useGravity = false;
    }

    public void KillPlayer() {
        Debug.Log("player killed");

        // disable controls
        gameObject.GetComponent<PlayerInput>().DeactivateInput();

        // TODO: check if carried bombs are detected within the explosion
        // if yes, then they should be exploded from the bomb script
        // if not, explode carried bomb here

        // death animation

        // setting player object to inactive makes a new one spawn when input is detected
        // so just render the player invisible and uncollidable
        gameObject.GetComponent<Collider>().enabled = false;
        gameObject.GetComponent<Renderer>().enabled = false;
    }

    void Update() {
        // move
        // apply translation to the world axes, so movement direction is constant and follows thumbsticks
        //Vector3 translation = new Vector3(moveVal.x / 2, 0, moveVal.y / 2);
        Vector3 movementTranslation = new Vector3(moveVal.x, 0, moveVal.y);
        // Debug.Log("movement" + movementTranslation);
        // transform.Translate(movementTranslation * moveSpeed * Time.deltaTime, Space.World);

        playerBody.AddForce(movementTranslation * moveSpeed, ForceMode.Impulse);
        // playerBody.velocity = new Vector3(moveVal.x *10, 0, moveVal.y*10);
        // playerBody.velocity = translation;

        // dash
        if (dashActivated) {
            dashActivated = false;

            // if character is moving, apply dash in direction of movement
            // otherwise if character is stationary, apply dash in direction that player is facing

            if (movementTranslation != Vector3.zero) {
                // transform.Translate(translation * dashDistance, Space.World);
                // Vector3 movementDirection = new Vector3(translation.x*2,0,translation.y*2);
                playerBody.AddForce(movementTranslation / 2 * dashDistance, ForceMode.Impulse);
            } else {
                // transform.Translate(Vector3.forward * dashDistance);
                Vector3 stationaryDirection = new Vector3(faceDirection.x / (3/2), 0, faceDirection.y / (3/2));
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

    // use OnCollisionStay to reconfirm object collision every frame
    void OnCollisionStay(Collision col) {
        if (col.gameObject.CompareTag("Bomb")) {
            System.String colliderName = col.GetContact(0).thisCollider.name;

            // can only pick up if bomb is from the front
            if (colliderName == "FrontCollider") {
                if (col.gameObject.GetComponent<BombController>().inAir) {
                    PickUpBomb(col.gameObject);
                } else pickableBomb = col.gameObject;
            }

            else if (colliderName == "SideBackCollider") {
                if (col.gameObject.GetComponent<BombController>().inAir) {
                    // explode bomb immediately
                } else pickableBomb = col.gameObject;
            }
        }
    }

    void OnCollisionExit(Collision col) {
        if (col.gameObject.CompareTag("Bomb")) {
            pickableBomb = null;
        }
    }
}
