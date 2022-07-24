using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class CommonController : MonoBehaviour {

    protected void EnableAllColliders(bool boolean) {
        Collider[] allColliders = gameObject.GetComponentsInChildren<Collider>();

        foreach(Collider collider in allColliders) {
            collider.enabled = boolean;
        }
    }

    protected void EnableAllRenderers(bool boolean) {
        Renderer[] allRenderers = gameObject.GetComponentsInChildren<Renderer>();

        foreach(Renderer renderer in allRenderers) {
            renderer.enabled = boolean;
        }
    }
}
