using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FireBombController : ExplosiveController {
    public GameObject fireFX;
    public int fireLifetime;

    protected GameObject fireObject;
    protected List<GameObject> fireList;

    public override IEnumerator ExplodeNow() {
        destroyed = true;
        DetachFromPlayer();
        explosionSound.PlayOneShot(explosionClip);

        if (explosionCircleObject) Destroy(explosionCircleObject);

        // if explosionFX not set in editor, don't play explosion FX
        GameObject explosion = null;
        if (explosionFX) explosion = Instantiate(explosionFX, transform.position, Quaternion.identity);

        // make the object invisible for now
        EnableAllColliders(false);
        EnableAllRenderers(false);

        CheckExplosionDamage();

        if (explosion) {
            // destroy FX object after it finishes
            yield return new WaitForSeconds(3f);
            Destroy(explosion);
        }
    }


    protected override void CheckExplosionDamage() {
        fireList = new List<GameObject>();

        Collider[] objectsInExplosion = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider other in objectsInExplosion) {
            if (other.tag == "Grass") {
                other.gameObject.GetComponent<Renderer>().enabled = false;

                fireObject = Instantiate(fireFX, other.gameObject.transform.position, Quaternion.identity);
                fireObject.GetComponent<GroundFireController>().setOwner(lastHeld);

                fireList.Add(fireObject);
            }
        }

        StartCoroutine(FireDelayDespawn());
    }

    // spawn fire FX on ground
    protected IEnumerator FireDelayDespawn() {
        yield return new WaitForSeconds(fireLifetime);

        foreach (GameObject fire in fireList) {
            Destroy(fire);
        }

        Destroy(gameObject);
    }
}
