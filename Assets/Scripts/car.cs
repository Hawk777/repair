using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class car : MonoBehaviour
{
    public float maxAccel = 2f;
    public float accel = 0.05f;
    public float naturalDecel = 0.02f;
    public float turnAngle = 1.5f;
    public float axleTurn = 30f;
    public float degreeOfFacing = 0f;
    public float currentAccel = 0f;
    public bool drift = false;
    public Vector2 force;
    public Vector2 driftPoint;

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
        int turn =0, forward=0;
        turn = (int)Input.GetAxisRaw("Horizontal");
        forward = (int)Input.GetAxisRaw("Vertical");
        if (Mathf.Abs(currentAccel + forward*accel) > maxAccel) {
            currentAccel = Mathf.Sign(currentAccel)*maxAccel;
        }
        else
        {
            currentAccel += forward*accel;
        }
        if (Mathf.Abs(currentAccel) < naturalDecel)
        {
            currentAccel = 0;
        }
        else { currentAccel = Mathf.Sign(currentAccel) * (Mathf.Abs(currentAccel) - naturalDecel); }
        
        if (currentAccel!=0) degreeOfFacing += -turn * turnAngle * 2 * Mathf.Min(maxAccel/2,currentAccel)/maxAccel;

        // push car by currentAccel units in the direction of its facing
        //transform.eulerAngles = new Vector3(0, 0, degreeOfFacing);
        force = new Vector2(Mathf.Cos((degreeOfFacing + turn * axleTurn) * Mathf.Deg2Rad), Mathf.Sin((degreeOfFacing + turn * axleTurn) * Mathf.Deg2Rad));
        rb2D.AddForce(force * currentAccel);
        rb2D.MoveRotation(degreeOfFacing + turn * turnAngle);
        /*
        if (drift == 0)
        {
            // ONLY do AddForce according to heading if we're NOT in drift mode!
            force = new Vector2(Mathf.Cos((degreeOfFacing + turn * axleTurn) * Mathf.Deg2Rad), Mathf.Sin((degreeOfFacing + turn * axleTurn) * Mathf.Deg2Rad));
            rb2D.AddForce(force*currentAccel);
            rb2D.MoveRotation(degreeOfFacing + turn * turnAngle);
            //WHEN WE ENTER DRIFT MODE, save the force on the car and keep applying it regardless of degree of facing.  Left and right changes it by turn*turnAngle as normal.
        }
        else
        {
            rb2D.AddForce(force*currentAccel);
            rb2D.MoveRotation(degreeOfFacing + turn * turnAngle);
        }
        */

        // when drift is pressed, pick a point some distance to the left/right, depending on rotational inertia.  Hold that point's coords in memory.
        // As long as drift is pressed, do AddForceAtPosition on that point, with a NEGATIVE force, to pull the car AROUND that point.  Maybe decrease drag/naturalDecel during drift.
        //No inertia means no drift, high inertia in a direction means high drift; inverse relationship, will have to play around w/ numbers.
    }
}
