using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ExplosiveController : CommonController {

    public GameObject explosionFX;
    public bool inAir = false;

    public float fuseDelay;
    public float explosionRadius;

    private bool activated = false;
    private bool destroyed = false;


    public bool isInAir() {
        return inAir;
    }

   public IEnumerator StartFuse() {
        activated = true;
        yield return new WaitForSeconds(fuseDelay);

        if (!destroyed) StartCoroutine(ExplosionEffect());
    }

    private IEnumerator ExplosionEffect() {
        destroyed = true;

        GameObject explosion = Instantiate(explosionFX, transform.position, Quaternion.identity);

        // make the object invisible for now
        DisableAllColliders();
        DisableAllRenderers();

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

                if (!isBlocked) {
                    col.gameObject.GetComponent<PlayerController>().KillPlayer();
                }
            } else if (col.tag == "Bomb" || col.tag == "Barrel") {
                StartCoroutine(col.gameObject.GetComponent<ExplosiveController>().StartFuse());
            }
        }
    }

    private void OnCollisionEnter(Collision col) {
        // check if activated i.e. bomb fuse is lit
        if (activated) {
            // explode if contact wall or another bomb
            if (col.gameObject.CompareTag("Wall")) {
                StartCoroutine(ExplosionEffect());
            }

            // explode if contact another activated bomb
            else if (col.gameObject.CompareTag("Bomb") && col.gameObject.GetComponent<ExplosiveController>().activated) {
                StartCoroutine(ExplosionEffect());
            }
        }
    }
    
    private void OnCollisionStay(Collision col) {
        if (col.gameObject.CompareTag("Ground")) {
            inAir = false;
        }
    }
}
