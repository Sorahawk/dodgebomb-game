using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    private Rigidbody playerBody;
    private Rigidbody bombBody;
    public GameObject bomb;

    // throwing
    public float rotationSpeed;
    public float transformSpeed;
    public float throwForce;
    public float throwUpwardForce;
    public bool isThrowable = true;
    private Vector3 playerDirection;

    // Start is called before the first frame update
    void Start()
    {
        // Set to be 30 FPS
        Application.targetFrameRate =  30;
        playerBody = GetComponent<Rigidbody>();
        bombBody = bomb.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // movement
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical);
        playerBody.AddForce(movement * speed); // player move
        if (isThrowable == true) 
        {
            bombBody.AddForce(movement * speed); // bomb move
        }

        // if player release control, stop
        if (Input.GetKeyUp("w") || Input.GetKeyUp("a") || Input.GetKeyUp("s") || Input.GetKeyUp("d")){
          playerBody.velocity = Vector3.zero;
          if (isThrowable == true)
          {
            bombBody.velocity = Vector3.zero;
            movement = Vector3.zero;
          }
        }

        // if player is moving, change rotation
        if (movement != Vector3.zero) 
        {
            // set direction for bomb throw
            playerDirection = movement;
            playerDirection.Normalize();

            // change player rotation
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);

            // change bomb position
            float horizontalBombOffset;
            float verticalBombOffset;

            if (moveHorizontal > 0) {
                horizontalBombOffset = (float) 1;
            } else if (moveHorizontal < 0) {
                horizontalBombOffset = (float) -1;
            } else {
                horizontalBombOffset = 0;
            }

            if (moveVertical > 0) {
                verticalBombOffset = (float) 1;
            } else if (moveVertical < 0) {
                verticalBombOffset = (float) -1;
            } else {
                verticalBombOffset = 0;
            }

            Vector3 targetPosition = transform.position + new Vector3(horizontalBombOffset, (float) -0.5, verticalBombOffset);
            bomb.transform.position = Vector3.MoveTowards(bomb.transform.position, targetPosition, transformSpeed);
            
        }

        // throw
        if (Input.GetKeyDown(KeyCode.Space) && isThrowable) 
        {
            ThrowBomb();
        }
    }

    void ThrowBomb()
    {
        isThrowable = false;
        Vector3 bombForce = playerDirection * throwForce + transform.up * throwUpwardForce; // playerBody.transform.right
        bombBody.AddForce(bombForce, ForceMode.Impulse);
    }
}
