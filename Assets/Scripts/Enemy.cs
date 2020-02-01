using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Rigidbody2D enemyBody;

    // Start is called before the first frame update
    void Start()
    {
        enemyBody = GetComponentInParent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // if the player is too far away, don't do anything.  If they are close enough to render, attack the nearest thing that can be attacked (either player or objectives
        GameObject player = GameManager.getPlayer();

        // TODO
        
    }
}
