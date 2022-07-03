using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireDebris : MonoBehaviour
{
    private  Rigidbody rigidBody;
    private  Vector3 scaler;

    // Start is called before the first frame update
    void  Start()
    {
        // we want the object to have a scale of 0 (disappear) after 30 frames. 
        scaler = transform.localScale / (float) 30 ;
        rigidBody = GetComponent<Rigidbody>();
        StartCoroutine("ScaleOut");
    }

    IEnumerator  ScaleOut(){

        Vector3 direction =  new  Vector3(Random.Range(-1.0f, 1.0f), 1, Random.Range(-1.0f, 1.0f));
        rigidBody.AddForce(direction.normalized  *  10, ForceMode.Impulse);
        // rigidBody.AddTorque(10, ForceMode.Impulse);
        // wait for next frame
        yield  return  null;

        // render for 0.5 second
        for (int step =  0; step  < 30; step++)
        {
            this.transform.localScale  =  this.transform.localScale  -  scaler;
            // wait for next frame
            yield  return  null;
        }

        Destroy(gameObject);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
