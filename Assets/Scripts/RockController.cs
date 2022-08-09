using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RockController : ExplosiveController {

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
                PlayerController playerScript = col.gameObject.GetComponent<PlayerController>();

                // drop any held bombs
                playerScript.DropBomb();

                // stun player
                playerScript.StunPlayer();
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
            }

            activated = false;
        }
    }
}
