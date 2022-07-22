using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BombController : MonoBehaviour {
    public GameObject explosionFX;
    private float explosionRadius = 3;

    public bool activated = false;
    public bool inAir = false;
    private bool destroyed = false;

    public IEnumerator StartFuse() {
        activated = true;
        yield return new WaitForSeconds(3.0f);

        if (!destroyed) StartCoroutine(ExplosionEffect());
    }

    IEnumerator ExplosionEffect() {
        destroyed = true;

        GameObject explosion = Instantiate(explosionFX, transform.position, Quaternion.identity);

        // make the object invisible for now
        gameObject.GetComponent<Collider>().enabled = false;
        DisableAllRenderers();

        CheckExplosionDamage(gameObject.transform.position, explosionRadius);

        // destroy FX object after it finishes
        yield return new WaitForSeconds(3.0f);
        Destroy(explosion);

        // destroy game object
        Destroy(gameObject);
    }

    void DisableAllRenderers() {
        Renderer[] allRenderers = gameObject.GetComponentsInChildren<Renderer>();

        foreach(Renderer renderer in allRenderers) {
            renderer.enabled = false;
        }
    }

    void CheckExplosionDamage(Vector3 position, float radius) {
        Collider[] objectsInExplosion = Physics.OverlapSphere(position, radius);

        foreach (Collider col in objectsInExplosion) {
            if (col.tag == "Player") {
                // this raycast will detect if there any colliders between the explosion origin and the player
                // all objects with colliders on the player itself have been placed on the Ignore Raycast layer to be ignored
                bool isBlocked = Physics.Linecast(transform.position, col.gameObject.transform.position);

                if (!isBlocked) {
                    col.gameObject.GetComponent<PlayerController>().KillPlayer();
                }
            } else if (col.tag == "Bomb") {
                StartCoroutine(col.gameObject.GetComponent<BombController>().StartFuse());
            } else if (col.tag == "Barrel") {
                StartCoroutine(col.gameObject.GetComponent<BarrelController>().ExplosionEffect());
            }
        }
    }

    void OnCollisionEnter(Collision col) {
        // check if activated i.e. bomb fuse is lit
        if (activated) {
            // explode if contact wall or another bomb
            if (col.gameObject.CompareTag("Wall")) {
                StartCoroutine(ExplosionEffect());
            }

            // explode if contact another activated bomb
            else if (col.gameObject.CompareTag("Bomb") && col.gameObject.GetComponent<BombController>().activated) {
                StartCoroutine(ExplosionEffect());
            }
        }
    }
    
    void OnCollisionStay(Collision col) {
        if (col.gameObject.CompareTag("Ground")) {
            inAir = false;
        }
    }
}
