using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassBombController : ExplosiveController {

    public new IEnumerator ExplodeNow() {
        destroyed = true;

        // if explosionFX not set in editor, don't play explosion FX
        GameObject explosion = null;
        if (explosionFX) explosion = Instantiate(explosionFX, transform.position, Quaternion.identity);

        // make the object invisible for now
        EnableAllColliders(false);
        EnableAllRenderers(false);

        CheckExplosionDamage();

        if (explosion) {
            // destroy FX object after it finishes
            yield return new WaitForSeconds(1f);
            Destroy(explosion);
        }

        // destroy game object
        Destroy(gameObject);
    }

    private new void CheckExplosionDamage() {
        Collider[] objectsInExplosion = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider other in objectsInExplosion) {
            if (other.tag == "Grass") {
                other.gameObject.GetComponent<Renderer>().enabled = true;
            }
        }
    }

    // just to override the inherited one
    private new void OnCollisionEnter(Collision col) {
    }

    private new void OnCollisionStay(Collision col) {
        if (activated) {
            Debug.Log(col.gameObject.tag);
            StartCoroutine(ExplodeNow());
        }
    }
}
