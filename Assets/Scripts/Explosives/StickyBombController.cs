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
        }
    }
}
