using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float damper = 0.1f;
    private Transform playerTransform;
    private Vector3 defaultPosition = new Vector3(0,0,-10);
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //transform.position = new Vector3(playerTransform.position.x,playerTransform.position.y,transform.position.z);
        //transform.rotation = playerTransform.rotation * Quaternion.Euler(0, 0, 0);
        if (SceneManager.GetActiveScene().name != "Start Menu" && SceneManager.GetActiveScene().name != "Game Over")
        {
            Vector2 temp = Vector2.Lerp(transform.position, playerTransform.position, damper);
            transform.position = new Vector3(temp.x, temp.y, transform.position.z);
            transform.rotation = Quaternion.Lerp(transform.rotation, playerTransform.rotation * Quaternion.Euler(0, 0, 0), damper);
        }
        else
        {
            transform.rotation = Quaternion.identity;
            transform.position = defaultPosition;
        }
    }
}
