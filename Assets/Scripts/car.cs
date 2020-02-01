﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class car : MonoBehaviour
{
    public float maxAccel = 2f;
    public float accel = 0.05f;
    public float naturalDecel = 0.02f;
    public float turnAngle = 0.2f;
    public float hyperTurnAngle = 0.4f;
    public float maxAngularVelocity = 30f;
    public float maxHyperAngularVelocity = 90f;
    public float axleTurn = 30f;
    public float degreeOfFacing = 0f;
    public float currentAccel = 0f;
    public bool drift = false;
    public Vector2 force;
    public Vector2 driftPoint; // the RELATIVE point around which we are drifting
    public bool lastFrameDrift = false;
    public int whichLiquid = 0;
    public int liquidAmount = 0;

    private float necessaryAngularVelocity; //how much angular velocity you need to start throwing juice
    //private JuiceLauncher launcher;
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
        drift = Input.GetButton("Fire1"); //default mapped to Ctrl.
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
        
        //if (currentAccel!=0) degreeOfFacing += -turn * turnAngle * 2 * Mathf.Min(maxAccel/2,currentAccel)/maxAccel;

        // push car by currentAccel units in the direction of its facing
        //transform.eulerAngles = new Vector3(0, 0, degreeOfFacing);
        //force = new Vector2(Mathf.Cos((degreeOfFacing + turn * axleTurn) * Mathf.Deg2Rad), Mathf.Sin((degreeOfFacing + turn * axleTurn) * Mathf.Deg2Rad));
        //rb2D.AddForce(force * currentAccel);
        //rb2D.MoveRotation(degreeOfFacing + turn * turnAngle * Mathf.Min(maxAccel,currentAccel)/maxAccel);
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

        // no more formal "drift", just a hyperturn, with a sharper turn that also sends a command to the turret/bowl to fire liquid in the opposite direction of the turn.
        // ie. a left turn throws liquid right, at a distance according to angular velocity.  Just send a message to the turret script to throw some goo, give angular velocity.
        //Maybe it only fires periodically instead of every frame.

        if (!drift)
        {
            if (currentAccel != 0) degreeOfFacing += -turn * hyperTurnAngle * 2 * Mathf.Min(maxAccel / 2, currentAccel) / maxAccel;

            // push car by currentAccel units in the direction of its facing
            //transform.eulerAngles = new Vector3(0, 0, degreeOfFacing);
            force = new Vector2(Mathf.Cos((rb2D.rotation - turn * axleTurn) * Mathf.Deg2Rad), Mathf.Sin((rb2D.rotation - turn * axleTurn) * Mathf.Deg2Rad));
            rb2D.AddForce(force * currentAccel);
            //rb2D.MoveRotation(degreeOfFacing + turn * turnAngle * Mathf.Min(maxAccel, currentAccel) / maxAccel);
            rb2D.AddTorque(-turn* turnAngle);
            if (rb2D.angularVelocity > maxAngularVelocity) rb2D.angularVelocity = maxAngularVelocity;
            if (rb2D.angularVelocity < -maxAngularVelocity) rb2D.angularVelocity = -maxAngularVelocity;
        }
        else
        {
            if (currentAccel != 0) degreeOfFacing += -turn * turnAngle * 2 * Mathf.Min(maxAccel / 2, currentAccel) / maxAccel;
            force = new Vector2(Mathf.Cos((rb2D.rotation + turn * axleTurn) * Mathf.Deg2Rad), Mathf.Sin((rb2D.rotation + turn * axleTurn) * Mathf.Deg2Rad));
            rb2D.AddForce(force * currentAccel);
            //rb2D.MoveRotation(degreeOfFacing + turn * turnAngle * Mathf.Min(maxAccel, currentAccel) / maxAccel);
            rb2D.AddTorque(-turn* hyperTurnAngle);
            if (rb2D.angularVelocity > maxHyperAngularVelocity) rb2D.angularVelocity = maxHyperAngularVelocity;
            if (rb2D.angularVelocity < -maxHyperAngularVelocity) rb2D.angularVelocity = -maxHyperAngularVelocity;
        }

        //TODO: check the tag of the road we're on, and change acceleration/velocity/angularVelocity accordingly
    }
}
