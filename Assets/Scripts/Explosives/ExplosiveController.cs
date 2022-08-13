using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ExplosiveController : CommonController {

    public Transform smokeTransform;
    public GameObject smokeFX;

    public GameObject explosionFX;
    public GameObject explosionCircle;

    public float fuseDelay;
    public float explosionRadius;

    public PlayerVariable player1Variable;
    public PlayerVariable player2Variable;
    public PlayerVariable player3Variable;
    public PlayerVariable player4Variable;
    public PlayerVariable player5Variable;
    public PlayerVariable player6Variable;
    private PlayerVariable[] playerVarList;
    private PlayerVariable playerVariable;

    protected GameObject smokeObject;
    protected GameObject explosionCircleObject;

    protected Rigidbody bombBody;
    protected Collider bombCollider;
    protected PlayerController playerScript;
    protected GameObject playerHolding = null;

    protected bool activated = false;
    protected bool inAir = false;
    protected bool destroyed = false;
    protected int lastHeld = -1;

    protected void Start() {
        bombBody = GetComponent<Rigidbody>();
        bombCollider = GetComponent<Collider>();
        playerVarList = new PlayerVariable[] {player1Variable, player2Variable, player3Variable, player4Variable, player5Variable, player6Variable};
    }

    public bool getInAir() {
        return inAir;
    }

    public void setInAir(bool boolean) {
        inAir = boolean;
    }

    public bool getActive() {
        return activated;
    }

    // bind the bomb to the character
    public void AttachToPlayer(GameObject playerObject, int playerIndex) {
        // if a player is already holding the bomb, detach it
        if (playerHolding) DetachFromPlayer();

        // attach bomb to new player
        playerHolding = playerObject;
        lastHeld = playerIndex;
        playerScript = playerHolding.GetComponent<PlayerController>();

        playerScript.SetCarryBomb(gameObject);

        // reset object rotation so it doesn't get skewed
        transform.rotation = Quaternion.identity;

        // attach bomb to player's bomb holder
        transform.SetParent(playerScript.bombContainer);
        transform.localPosition = Vector3.zero;

        // turn on bomb kinematics so position is fixed
        bombBody.isKinematic = true;

        // turn off bomb collider so it doesn't affect the physics if it touches anything
        bombCollider.enabled = false;
    }

    // detach bomb from current player object holding it
    public void DetachFromPlayer() {
        if (!playerHolding) return;

        playerScript.SetCarryBomb(null);
        playerScript = null;
        playerHolding = null;

        // turn off bomb kinematics
        bombBody.isKinematic = false;

        // turn on collider
        bombCollider.enabled = true;

        // detach bomb
        transform.SetParent(null);
    }

    // set bomb to active
    public virtual void ActivateBomb() {
        activated = true;

        // set fuseDelay to -1 in editor for it to explode ONLY on contact with any collider
        if (fuseDelay >= 0) StartCoroutine(StartFuse());
    }

    // start fuse timer
    public virtual IEnumerator StartFuse() {
        if (fuseDelay > 0) {
            // smoke VFX
            if (smokeFX && !smokeObject) {
                smokeObject = Instantiate(smokeFX, smokeTransform.position, Quaternion.identity);
                smokeObject.transform.SetParent(smokeTransform);
                smokeObject.transform.localPosition = Vector3.zero;
            }

            // explosion radius VFX
            // start fuse can be called multiple times, but only spawn the circle once
            if (!explosionCircleObject) {
                explosionCircleObject = Instantiate(explosionCircle, bombBody.position, Quaternion.identity);
                explosionCircleObject.transform.localScale = new Vector3(explosionRadius * 2, explosionRadius * 2, explosionRadius * 2);
                explosionCircleObject.transform.localRotation = Quaternion.Euler(90, 0, 0);
            }

            yield return new WaitForSeconds(fuseDelay);
        }

        if (!destroyed) StartCoroutine(ExplodeNow());
    }

    private void Update() {
        if (explosionCircleObject) {
            Vector3 bombPosition = bombBody.transform.position;
            Vector3 circleFollowPos = new Vector3(bombPosition.x, bombPosition.y + 0.1f, bombPosition.z);
            explosionCircleObject.transform.position = circleFollowPos;
        }
    }

    // explode immediately
    public virtual IEnumerator ExplodeNow() {
        destroyed = true;
        DetachFromPlayer();

        if (explosionCircleObject) Destroy(explosionCircleObject);

        // if explosionFX not set in editor, don't play explosion FX
        GameObject explosion = null;
        if (explosionFX) explosion = Instantiate(explosionFX, transform.position, Quaternion.identity);

        // make the object invisible for now
        EnableAllColliders(false);
        EnableAllRenderers(false);

        CheckExplosionDamage();

        if (explosion) {
            // destroy FX object after it finishes
            yield return new WaitForSeconds(3);
            Destroy(explosion);
        }

        // destroy game object
        Destroy(gameObject);
    }

    protected virtual void CheckExplosionDamage() {
        Collider[] objectsInExplosion = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider other in objectsInExplosion) {
            if (other.tag == "Player") {
                // this raycast will detect if there any colliders between the explosion origin and the player
                // all objects with colliders on the player itself have been placed on the Ignore Raycast layer to be ignored
                bool isBlocked = Physics.Linecast(transform.position, other.gameObject.transform.position);
                PlayerController playerScript = other.GetComponent<PlayerController>();

                if (!isBlocked) {
                    // disable shield if have
                    if (playerScript.CheckShield()) playerScript.DisableShield();

                    // if no shield
                    else {
                        if (other.gameObject.GetComponent<PlayerController>().playerInput.playerIndex == lastHeld) {
                            MinusScore(lastHeld);
                        } else IncreaseScore(lastHeld);

                        other.gameObject.GetComponent<PlayerController>().KillPlayer();
                    }
                }
            }

            else if (other.tag == "Bomb" || other.tag == "Barrel") {
                StartCoroutine(other.gameObject.GetComponent<ExplosiveController>().StartFuse());

                if (other.gameObject.GetComponent<ExplosiveController>().GetLastHeld() == -1) {
                    other.gameObject.GetComponent<ExplosiveController>().SetLastHeld(lastHeld);
                }

                // additionally apply explosion force on bombs
                if (other.tag == "Bomb") {
                    other.gameObject.GetComponent<Rigidbody>().AddExplosionForce(explosionRadius * 3, transform.position, explosionRadius, 0, ForceMode.Impulse);
                }
            }

            else if (other.tag == "StickyBomb") {
                other.gameObject.GetComponent<StickyBombController>().ActivateBomb();

                if (other.gameObject.GetComponent<ExplosiveController>().GetLastHeld() == -1) {
                    other.gameObject.GetComponent<ExplosiveController>().SetLastHeld(lastHeld);
                }
            }

            else if (other.tag == "Rock") {
                other.gameObject.GetComponent<RockController>().ExplodeNow();
            }

            else if (other.tag == "Grass") {
                other.gameObject.GetComponent<Renderer>().enabled = false;
            }
        }
    }

    protected void MinusScore(int playerIndex) {
        playerVariable = playerVarList[playerIndex];

        if (playerVariable.Score > 0) {
            playerVariable.ApplyScoreChange(-1);
        }
    }

    protected void IncreaseScore(int playerIndex) {
        playerVariable = playerVarList[playerIndex];
        playerVariable.ApplyScoreChange(1);
    }

    public int GetLastHeld() {
        return lastHeld;
    }

    public void SetLastHeld(int index) {
        lastHeld = index;
    }

    protected virtual void OnCollisionEnter(Collision col) {
        // check if activated i.e. bomb fuse is lit
        if (activated) {
            // explode on contact with anything if fuseDelay is negative
            if (fuseDelay < 0) StartCoroutine(ExplodeNow());

            // explode if contact wall
            // if (col.gameObject.CompareTag("Wall")) StartCoroutine(ExplodeNow());

            // explode if contact another activated bomb
            if (col.gameObject.CompareTag("Bomb") && col.gameObject.GetComponent<ExplosiveController>().getActive()) {
                StartCoroutine(ExplodeNow());
            }
        }
    }
    
    protected virtual void OnCollisionStay(Collision col) {
        if (col.gameObject.CompareTag("Ground")) inAir = false;
    }

    protected virtual void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("OutOfBounds")) {
            if (explosionCircleObject) Destroy(explosionCircleObject);
            Destroy(gameObject);
        }
    }
}
