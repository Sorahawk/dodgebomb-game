using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RockController : ExplosiveController {
    public new void ExplodeNow() {
        // destroy game object
        Destroy(gameObject);
    }

    protected override void OnCollisionEnter(Collision col) {
        if (activated) {
            GameObject other = col.gameObject;

            if (other.CompareTag("Player")) {
                PlayerController playerScript = other.GetComponent<PlayerController>();
                explosionSound.PlayOneShot(explosionClip);

                if (playerScript.CheckShield()) playerScript.DisableShield();

                // stun and disarm player
                else playerScript.StunPlayer();
            }

            else if (other.CompareTag("Bomb") || other.CompareTag("StickyBomb") || other.CompareTag("Barrel")) {
                ExplosiveController bombScript = other.GetComponent<ExplosiveController>();
                explosionSound.PlayOneShot(explosionClip);

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

    protected override void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("OutOfBounds")) {
            Destroy(gameObject);
        }
    }
}
