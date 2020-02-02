using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform playerTransform;
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = new Vector3(playerTransform.position.x,playerTransform.position.y,transform.position.z);

        transform.rotation = new Quaternion(transform.rotation.x,transform.rotation.y,(playerTransform.rotation.z+90f)%360,transform.rotation.w);
    }
}
