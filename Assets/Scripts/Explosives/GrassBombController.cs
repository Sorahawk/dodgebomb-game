using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassBombController : ExplosiveController {
    protected override void CheckExplosionDamage() {
        Collider[] objectsInExplosion = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider other in objectsInExplosion) {
            if (other.tag == "Grass") {
                other.gameObject.GetComponent<Renderer>().enabled = true;
            }
        }
    }

    protected override void OnCollisionStay(Collision col) {
        if (activated) {
            StartCoroutine(ExplodeNow());
        }
    }

    protected override void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("OutOfBounds") || other.gameObject.CompareTag("Fire")) {
            Destroy(gameObject);
        }
    }
}
