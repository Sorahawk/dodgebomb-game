using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BearTrapController : CommonController {
    public Transform FXcontainer;
    public GameObject bloodFX;
    protected GameObject trap;
    public AudioSource activateSound;
    private int owner = -1;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            if (owner != other.gameObject.GetComponent<PlayerController>().playerInput.playerIndex) {
                EnableAllColliders(false);
                StartCoroutine(activateTrap());
            }
        }
    }

    public void setOwner(int playerIndex) {
        owner = playerIndex;
    }

    public int getOwner() {
        return owner;
    }

    public IEnumerator activateTrap() {
        activateSound.PlayOneShot(activateSound.clip);

        trap = Instantiate(bloodFX, FXcontainer.position, Quaternion.identity);
        trap.transform.SetParent(FXcontainer);
        trap.transform.localPosition = Vector3.zero;

        // destroy FX object after it finishes
        yield return new WaitForSeconds(0.5f);
        Destroy(trap);
        
        // destroy game object
        Destroy(gameObject);
    }
}
