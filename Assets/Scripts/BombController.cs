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
        gameObject.GetComponent<Collider>().enabled = false;
        gameObject.GetComponent<Renderer>().enabled = false;

        CheckExplosionDamage(gameObject.transform.position, explosionRadius);

        // destroy FX object after it finishes
        yield return new WaitForSeconds(3.0f);
        Destroy(explosion);

        // destroy bomb game object
        Destroy(gameObject);
    }

    void CheckExplosionDamage(Vector3 position, float radius) {
        Collider[] objectsInExplosion = Physics.OverlapSphere(position, radius);

        foreach (Collider col in objectsInExplosion) {
            if (col.tag == "Player") {
                col.gameObject.GetComponent<PlayerController>().KillPlayer();
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
