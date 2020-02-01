using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class betterCar : MonoBehaviour
{
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
        
    }
}
