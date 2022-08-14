using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class IceBombController : ExplosiveController {
    protected override void CheckExplosionDamage() {
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


            else if (other.tag == "Bomb" || other.tag == "Barrel") {
                StartCoroutine(other.gameObject.GetComponent<ExplosiveController>().StartFuse());

                if (other.gameObject.GetComponent<ExplosiveController>().GetLastHeld() == -1) {
                    other.gameObject.GetComponent<ExplosiveController>().SetLastHeld(lastHeld);
                }

                // additionally apply explosion force on bombs
                if (other.tag == "Bomb") {
                    other.gameObject.GetComponent<Rigidbody>().AddExplosionForce(explosionRadius * 3, transform.position, explosionRadius, 0, ForceMode.Impulse);
                }
            }

            else if (other.tag == "StickyBomb") {
                other.gameObject.GetComponent<StickyBombController>().ActivateBomb();

                if (other.gameObject.GetComponent<ExplosiveController>().GetLastHeld() == -1) {
                    other.gameObject.GetComponent<ExplosiveController>().SetLastHeld(lastHeld);
                }
            }
        }
    }
}
