using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BearTrapController : MonoBehaviour {
    public Transform FXcontainer;
    public GameObject sparkFX;
    protected GameObject trap;
    public AudioSource activateSound;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            StartCoroutine(activateTrap());
        }
    }

    public IEnumerator activateTrap() {
        StartCoroutine(playSound());
        trap = Instantiate(sparkFX, FXcontainer.position, Quaternion.identity);
        trap.transform.SetParent(FXcontainer);
        trap.transform.localPosition = Vector3.zero;

        // destroy FX object after it finishes
        yield return new WaitForSeconds(0.5f);
        Destroy(trap);
        
        // destroy game object
        Destroy(gameObject);
    }

    public IEnumerator playSound()
    {
        activateSound.time=0.4f;
        activateSound.PlayOneShot(activateSound.clip);
        yield return new WaitForSeconds(activateSound.clip.length);
    }
}
