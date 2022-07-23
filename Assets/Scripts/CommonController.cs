using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class CommonController : MonoBehaviour {

    protected void DisableAllColliders() {
        Collider[] allColliders = gameObject.GetComponentsInChildren<Collider>();

        foreach(Collider collider in allColliders) {
            collider.enabled = false;
        }
    }

    protected void DisableAllRenderers() {
        Renderer[] allRenderers = gameObject.GetComponentsInChildren<Renderer>();

        foreach(Renderer renderer in allRenderers) {
            renderer.enabled = false;
        }
    }
}
