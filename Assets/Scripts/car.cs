using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class car : MonoBehaviour
{
    public float maxAccel = 3000f;
    public float accel = 100f;
    public float naturalDecel = 40f;
    public float turnAngle = 1f;
    public float hyperTurnAngle = 2f;
    public float maxAngularVelocity = 30f;
    public float maxHyperAngularVelocity = 90f;
    public float axleTurn = 30f;
    public float currentAccel = 0f;
    public bool drift = false;
    public Vector2 force; // the RELATIVE point around which we are drifting
    public GameObject minimapSprite;

    static int maxHealth = 5;
    public int currentHealth;
    static uint defaultInvuln = 120;
    uint invuln = 0;
    static int damagingVelocity = 30;

    private float necessaryAngularVelocity; //how much angular velocity you need to start throwing juice
    private BoxCollider2D boxCollider; // don't forget to put everything that can collide with the car on the same layer
    private Rigidbody2D rb2D;
    private Transform liquid;
    private JuiceLauncher launcher;
    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        liquid = transform.Find("carliquid");
        launcher = GetComponent<JuiceLauncher>();
        GameObject mini = Instantiate(minimapSprite, transform.position, Quaternion.identity, transform);
        mini.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 1);
        currentHealth = 5;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        drift = Input.GetButton("Fire1"); //default mapped to Ctrl.
        // if we're using a controller with variable input (like a gamepad axis), change the int below to float
        int turn =0, forward=0;
        turn = (int)Input.GetAxisRaw("Horizontal");
        forward = (int)Input.GetAxisRaw("Vertical");
        turn *= (int)Mathf.Sign(forward);

        if (forward == 0) {
            currentAccel = Mathf.Sign(currentAccel) * Mathf.Max(Mathf.Abs(currentAccel) - naturalDecel, 0);
        } else if (currentAccel != 0 && Mathf.Sign(forward) != Mathf.Sign(currentAccel)) {
            currentAccel = currentAccel + forward * accel - Mathf.Sign(currentAccel) * naturalDecel;
        } else {
            currentAccel = Mathf.Sign(currentAccel) * Mathf.Min(Mathf.Abs(currentAccel + (forward * accel)), maxAccel);
        }

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

        force = new Vector2(-Mathf.Sin((rb2D.rotation - turn * axleTurn) * Mathf.Deg2Rad), Mathf.Cos((rb2D.rotation - turn * axleTurn) * Mathf.Deg2Rad));
        rb2D.AddForce(force * currentAccel);
        float angle = drift ? hyperTurnAngle : turnAngle;
        float maxAV = drift ? maxHyperAngularVelocity : maxAngularVelocity;

        rb2D.AddTorque(-turn * angle * rb2D.mass);
        if (rb2D.angularVelocity > maxAV) rb2D.angularVelocity = maxAV;
        else if (rb2D.angularVelocity < -maxAV) rb2D.angularVelocity = -maxAV;

        liquid.localScale = new Vector3(0.5f * launcher.tankLevel / launcher.tankCapacity, 0.5f * launcher.tankLevel / launcher.tankCapacity, 1);
        liquid.GetComponent<SpriteRenderer>().color = launcher.juiceType.color;

        if (invuln < 0) invuln--;
        //TODO: check the tag of the road we're on, and change acceleration/velocity/angularVelocity accordingly
    }

    public bool attackable
    {
        get
        {
            return currentHealth != 0;
        }
    }

    void TakeDamage()
    {
        if (invuln == 0)
        {
            currentHealth--;
            invuln = defaultInvuln;
            if (currentHealth <= 0)
            {
                // play explosion, set game over
            }
            else if (currentHealth <= 2)
            {
                // play heavy smoke on the car
            }
            else if (currentHealth <= 4)
            {
                // play normal smoke on the car
            }
        }
    }
    /*
    void OnCollisionEnter2D (Collision2D collision)
    {
        if (rb2D.velocity > damagingVelocity || collision.gameObject.tag == "Enemy")
        {
            rb2D.AddForceAtPosition(new Vector2(0, 100), collision.GetContact);
        }
    }
    */
}
