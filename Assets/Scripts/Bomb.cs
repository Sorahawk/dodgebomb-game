using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public GameObject bomb;
    public GameObject fireDebris;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(BombExplodeAfterThreeSeconds());
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Wall")){
            // instant explode
            Destroy(bomb);
            
            // assume we have 5 debris per box
            for (int x =  0; x<5; x++){
                Instantiate(fireDebris, transform.position, Quaternion.identity);
            }
            // gameObject.transform.parent.GetComponent<SpriteRenderer>().enabled  =  false;
            gameObject.GetComponent<SphereCollider>().enabled  =  false;
            // GetComponent<EdgeCollider2D>().enabled  =  false;
        }
    }

    IEnumerator BombExplodeAfterThreeSeconds()
    {
        yield return new WaitForSeconds (3.0f);

        // instant explode
        Destroy(bomb);
        
        // assume we have 5 debris per box
        for (int x =  0; x<5; x++){
            Instantiate(fireDebris, transform.position, Quaternion.identity);
        }

        gameObject.GetComponent<SphereCollider>().enabled  =  false;
    }
}
