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
            GameObject other = col.gameObject;

            // disarm players
            if (other.CompareTag("Player")) {
                PlayerController playerScript = other.GetComponent<PlayerController>();

                // drop any held bombs
                playerScript.DropBomb();

                // stun player
                playerScript.StunPlayer();
            }

            else if (other.CompareTag("Bomb") || other.CompareTag("Barrel")) {
                ExplosiveController bombScript = other.GetComponent<ExplosiveController>();

                if (bombScript.GetLastHeld() == -1) bombScript.SetLastHeld(lastHeld);

                if (other.CompareTag("Barrel")) StartCoroutine(bombScript.StartFuse());
                else {
                    // activate inactive bombs
                    if (!bombScript.getActive()) {
                        if (bombScript.GetLastHeld() == -1) bombScript.SetLastHeld(lastHeld);

                        bombScript.ActivateBomb();
                    }
                }
            }

            activated = false;
        }
    }
}
