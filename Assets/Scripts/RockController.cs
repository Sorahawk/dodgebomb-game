using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RockController : ExplosiveController {

    // set bomb to active
    public new void ActivateBomb() {
        activated = true;
    }

    // explode immediately
    public new void ExplodeNow() {
        destroyed = true;

        // destroy game object
        Destroy(gameObject);
    }

    private new void OnCollisionEnter(Collision col) {
        if (activated) {
            // disarm players
            if (col.gameObject.CompareTag("Player")) {
                col.gameObject.GetComponent<PlayerController>().DropBomb();
            }

            // activate barrels
            else if (col.gameObject.CompareTag("Barrel")) {
                StartCoroutine(col.gameObject.GetComponent<ExplosiveController>().StartFuse());
            }

            else if (col.gameObject.CompareTag("Bomb")) {
                // activate inactive bombs
                if (!col.gameObject.GetComponent<ExplosiveController>().getActive()) {
                    col.gameObject.GetComponent<ExplosiveController>().ActivateBomb();
                }

                // blow up immediately if bomb is already activated
                else StartCoroutine(col.gameObject.GetComponent<ExplosiveController>().ExplodeNow());
            }
        }
    }
}
