using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float damper = 0.1f;
    private Transform playerTransform;
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = new Vector3(playerTransform.position.x,playerTransform.position.y,transform.position.z);

        transform.rotation = playerTransform.rotation * Quaternion.Euler(0,0,-90);
    }
}
