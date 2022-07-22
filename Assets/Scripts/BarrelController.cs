using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BarrelController : MonoBehaviour {
    public GameObject explosionFX;
    private float explosionRadius = 5;

    public IEnumerator ExplosionEffect() {
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
}
