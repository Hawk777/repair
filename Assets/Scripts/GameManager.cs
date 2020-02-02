using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    Camera mainCamera;
    Enemy[] enemies;
    static GameObject player;
    static Rigidbody2D playerBody;

    public static GameObject getPlayer()
    {
        return player;
    }
    public static Rigidbody2D getPlayerBody()
    {
        return playerBody;
    }

    void Start()
    {
        // make and place enemies and objectives

        mainCamera = FindObjectOfType<Camera>();
        player = GameObject.FindWithTag("Player");
        playerBody = player.GetComponent<Rigidbody2D>();

        // make colors.  these colors implicitly do different things
    }

    void Update()
    {
        // update enemies.  if dispensaries and objectives are time-based, update those too.
    }
}
