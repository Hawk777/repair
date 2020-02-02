using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dispensary : MonoBehaviour
{
    // if the player is within a certain distance of the dispensary, start pumping them full of the proper liquid.
    Rigidbody2D dispBody;
    public float flowDistance = 4;
    public Material liquid;
    public GameObject player;

    private SpriteRenderer render;
    private JuiceLauncher launcher;
    private BoxCollider2D collider;

    void Start()
    {
        dispBody = GetComponentInParent<Rigidbody2D>();
        render = GetComponent<SpriteRenderer>();
        collider = GetComponent<BoxCollider2D>();
        
        // spout is on the relative right of the dispenser, so check relative distance to player
        // change liquid color according to liquid
        render.color = liquid.color;
        //launcher = player.GetComponent<JuiceLauncher>();
    }

    void FixedUpdate()
    {

    }

    void OnTriggerStay2D()
    {
        launcher = FindObjectOfType<JuiceLauncher>();
        fill();
    }
    void fill()
    {
        if (launcher.juiceType != liquid) launcher.tankLevel--;
        if (launcher.tankLevel == 0) launcher.juiceType = liquid;
        if (launcher.juiceType == liquid)launcher.tankLevel++;
        if (launcher.tankLevel > launcher.tankCapacity)launcher.tankLevel = launcher.tankCapacity;
        
    }
}
