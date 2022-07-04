using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BombController : MonoBehaviour {
    public GameObject explosionFX;

    void OnCollisionEnter(Collision col) {
        if (col.gameObject.CompareTag("Wall")) {
            StartCoroutine(ExplosionEffect());
        }
    }

    public IEnumerator BombExplodeAfterThreeSeconds() {
        yield return new WaitForSeconds(3.0f);
        StartCoroutine(ExplosionEffect());
    }

    IEnumerator ExplosionEffect() {
        // destroy bomb game object
        Destroy(gameObject);

        GameObject explosion = Instantiate(explosionFX, transform.position, Quaternion.identity);

        yield return new WaitForSeconds(3.0f);
        Destroy(explosion);
    }
}
