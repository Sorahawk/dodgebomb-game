using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StickyBombController : ExplosiveController {
    protected override void OnCollisionEnter(Collision col) {
        if (activated) {
            // reset object rotation so it doesn't get skewed
            transform.rotation = Quaternion.identity;

            // stick to the first thing it touches after being activated
            transform.SetParent(col.gameObject.transform);

            // turn on kinematics and turn off collider
            bombBody.isKinematic = true;
            bombCollider.enabled = false;
            if (col.gameObject.CompareTag("Player")) {
                stickTo = col.gameObject.GetComponent<PlayerController>().playerInput.playerIndex;
            }
        }
    }

    protected override void CheckExplosionDamage() {
        Collider[] objectsInExplosion = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider other in objectsInExplosion) {
            if (other.tag == "Player") {
                // this raycast will detect if there any colliders between the explosion origin and the player
                // all objects with colliders on the player itself have been placed on the Ignore Raycast layer to be ignored
                bool isBlocked = Physics.Linecast(transform.position, other.gameObject.transform.position);
                PlayerController playerScript = other.GetComponent<PlayerController>();

                if (!isBlocked) {
                    // disable shield if have
                    if (playerScript.CheckShield()) playerScript.DisableShield();

                    // if no shield
                    else {
                        if (stickTo == -1) {
                            //stick to obstacles
                            if (other.gameObject.GetComponent<PlayerController>().playerInput.playerIndex != lastHeld) {
                                IncreaseScore(lastHeld);
                            }

                            other.gameObject.GetComponent<PlayerController>().KillPlayer();
                        } else {
                            if (other.gameObject.GetComponent<PlayerController>().playerInput.playerIndex == stickTo) {
                                // suicide. sticky bomb stick to self
                                if (other.gameObject.GetComponent<PlayerController>().playerInput.playerIndex != lastHeld) {
                                    IncreaseScore(lastHeld);
                                }
                            } else IncreaseScore(stickTo);

                            other.gameObject.GetComponent<PlayerController>().KillPlayer();
                        }
                    }
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
                other.gameObject.GetComponent<ExplosiveController>().ActivateBomb();

                if (other.gameObject.GetComponent<ExplosiveController>().GetLastHeld() == -1) {
                    other.gameObject.GetComponent<ExplosiveController>().SetLastHeld(lastHeld);
                }
            }

            else if (other.tag == "Rock") {
                other.gameObject.GetComponent<RockController>().ExplodeNow();
            }

            else if (other.tag == "Grass") {
                other.gameObject.GetComponent<Renderer>().enabled = false;
            }
        }
    }
}
