using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BearTrapController : CommonController {
    public Transform FXcontainer;
    public GameObject sparkFX;
    protected GameObject trap;
    public AudioSource activateSound;
    private bool active = true;
    private int owner = -1;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            if (owner != other.gameObject.GetComponent<PlayerController>().playerInput.user.id - 1) {
                EnableAllColliders(false);
                StartCoroutine(activateTrap());
            }
        }
    }

    public void setOwner(int playerIndex) {
        owner = playerIndex;
        print(owner);
    }

    public int getOwner() {
        return owner;
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
