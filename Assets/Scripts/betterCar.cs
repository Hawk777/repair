using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class betterCar : MonoBehaviour
{
    public int maxAngularVelocity = 15;
    public bool drift = false;
    public int axleTurn=30;
    public Vector2 force;

    private BoxCollider2D boxCollider; // don't forget to put everything that can collide with the car on the same layer
    private Rigidbody2D rb2D;
    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        drift = Input.GetButtonDown("Fire1"); //default mapped to Ctrl.
        // if we're using a controller with variable input (like a gamepad axis), change the int below to float
        int turn = 0, forward = 0;
        turn = (int)Input.GetAxisRaw("Horizontal");
        forward = (int)Input.GetAxisRaw("Vertical");

        force = new Vector2(Mathf.Cos((rb2D.rotation + turn * axleTurn) * Mathf.Deg2Rad), Mathf.Sin((rb2D.rotation + turn * axleTurn) * Mathf.Deg2Rad));
        rb2D.AddForce(force * forward);
        if (turn>0)
        {
            rb2D.AddTorque(turn * Mathf.Abs(Mathf.Clamp(maxAngularVelocity - rb2D.angularVelocity, 0, 15)));
        }
        else if (turn<0)
        {
            rb2D.AddTorque(turn * Mathf.Abs(Mathf.Clamp(-maxAngularVelocity - rb2D.angularVelocity, -15, 0)));
        }
        
    }
}
