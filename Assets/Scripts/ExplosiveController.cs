using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ExplosiveController : CommonController {

    public Transform smokeTransform;
    public GameObject smokeFX;
    public GameObject explosionFX;

    public float fuseDelay;
    public float explosionRadius;

    private Rigidbody bombBody;
    private Collider bombCollider;
    private GameObject playerHolding = null;

    private bool activated = false;
    private bool inAir = false;
    private bool destroyed = false;


    private void Start() {
        bombBody = GetComponent<Rigidbody>();
        bombCollider = GetComponent<Collider>();
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
    public void AttachToPlayer(GameObject playerObject) {
        // if a player is already holding the bomb, detach it
        if (playerHolding) DetachFromPlayer();

        // attach bomb to new player
        playerHolding = playerObject;
        transform.SetParent(playerObject.GetComponent<PlayerController>().bombContainer);
        transform.localPosition = Vector3.zero;

        // turn on bomb kinematics so position is fixed
        bombBody.isKinematic = true;

        // turn off bomb collider so it doesn't affect the physics if it touches anything
        bombCollider.enabled = false;
    }

    // detach bomb from current player object holding it
    public void DetachFromPlayer() {
        playerHolding.GetComponent<PlayerController>().SetCarryNull();
        playerHolding = null;

        // turn off bomb kinematics
        bombBody.isKinematic = false;

        // turn on collider
        bombCollider.enabled = true;

        // detach bomb
        transform.SetParent(null);
    }

    // set bomb to active
    public void ActivateBomb() {
        activated = true;

        // set fuseDelay to -1 in editor for it to explode ONLY on contact with any collider
        if (fuseDelay >= 0) StartCoroutine(StartFuse());
    }

    // start fuse timer
    public IEnumerator StartFuse() {
        if (fuseDelay > 0) {
            //GameObject smokeObject = Instantiate(smokeFX, smokeTransform.position, Quaternion.identity);
            //smokeObject.transform.SetParent(smokeTransform);
            //smokeObject.transform.localPosition = Vector3.zero;

            yield return new WaitForSeconds(fuseDelay);

            //Destroy(smokeObject);
        }

        if (!destroyed) StartCoroutine(ExplodeNow());
    }

    // explode immediately
    private IEnumerator ExplodeNow() {
        destroyed = true;

        GameObject explosion = Instantiate(explosionFX, transform.position, Quaternion.identity);

        // make the object invisible for now
        EnableAllColliders(false);
        EnableAllRenderers(false);

        CheckExplosionDamage(gameObject.transform.position, explosionRadius);

        // destroy FX object after it finishes
        yield return new WaitForSeconds(3.0f);
        Destroy(explosion);

        // destroy game object
        Destroy(gameObject);
    }

    private void CheckExplosionDamage(Vector3 position, float radius) {
        Collider[] objectsInExplosion = Physics.OverlapSphere(position, radius);

        foreach (Collider col in objectsInExplosion) {
            if (col.tag == "Player") {
                // this raycast will detect if there any colliders between the explosion origin and the player
                // all objects with colliders on the player itself have been placed on the Ignore Raycast layer to be ignored
                bool isBlocked = Physics.Linecast(transform.position, col.gameObject.transform.position);

                if (!isBlocked) col.gameObject.GetComponent<PlayerController>().KillPlayer();
            }

            else if (col.tag == "Bomb" || col.tag == "Barrel") {
                StartCoroutine(col.gameObject.GetComponent<ExplosiveController>().StartFuse());
            }
        }
    }

    private void OnCollisionEnter(Collision col) {
        // check if activated i.e. bomb fuse is lit
        if (activated) {

            // explode on contact with anything if fuseDelay is negative
            if (fuseDelay < 0) StartCoroutine(ExplodeNow());

            // explode if contact wall
            if (col.gameObject.CompareTag("Wall")) StartCoroutine(ExplodeNow());

            // explode if contact another activated bomb
            if (col.gameObject.CompareTag("Bomb") && col.gameObject.GetComponent<ExplosiveController>().getActive()) {
                StartCoroutine(ExplodeNow());
            }
        }
    }
    
    private void OnCollisionStay(Collision col) {
        if (col.gameObject.CompareTag("Ground")) inAir = false;
    }
}
