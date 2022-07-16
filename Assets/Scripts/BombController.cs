using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BombController : MonoBehaviour {
    public GameObject explosionFX;
    private bool activated = false;

    void OnCollisionEnter(Collision col) {
        // check if activated
        if (activated) {
            // explode if contact wall or another bomb
            if (col.gameObject.CompareTag("Wall") || col.gameObject.CompareTag("Bomb")) {
                StartCoroutine(ExplosionEffect());
            }
        }
    }

    public IEnumerator StartFuse() {
        // TODO: overhaul this script because throwing a bomb will start the fuse but when impact wall it will destroy the object
        // so will have null reference warning error

        activated = true;

        yield return new WaitForSeconds(3.0f);
        StartCoroutine(ExplosionEffect());
    }

    IEnumerator ExplosionEffect() {
        // destroy bomb game object
        Destroy(gameObject);

        GameObject explosion = Instantiate(explosionFX, transform.position, Quaternion.identity);

        // destroy FX object after it finishes
        // TODO: fix this cuz doesn't work
        yield return new WaitForSeconds(3.0f);
        Destroy(explosion);
    }
}
