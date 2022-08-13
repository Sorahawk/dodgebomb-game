using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class IceBombController : ExplosiveController {
    protected new void CheckExplosionDamage() {
        Collider[] objectsInExplosion = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider other in objectsInExplosion) {
            if (other.tag == "Player") {
                bool isBlocked = Physics.Linecast(transform.position, other.gameObject.transform.position);

                // check for shield
                if (other.GetComponent<PlayerController>().CheckShield()){
                    // disable shield if there is
                    other.GetComponent<PlayerController>().DisableShield();
                } else {
                    if (!isBlocked) other.gameObject.GetComponent<PlayerController>().StunPlayer();
                }
            }
        }
    }
}
