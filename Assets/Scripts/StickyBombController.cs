using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StickyBombController : ExplosiveController {
    protected new void OnCollisionEnter(Collision col) {
        if (activated) {
            // stick to the first thing it touches after being activated
            transform.SetParent(col.gameObject.transform);

            // turn on kinematics and turn off collider
            bombBody.isKinematic = true;
            bombCollider.enabled = false;
        }
    }
}
