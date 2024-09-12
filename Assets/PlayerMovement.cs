using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Controls velocity multiplier
    public float speed = 10f;
    private float moveInput;

    // Tells script there is a rigidbody,
    // we can use variable rb to reference it in further script
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        // //rb equals the rigidbody on the player
        rb = GetComponent<Rigidbody2D>();
    }


    // Update is called once per frame
    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        // Creates velocity in direction of value equal to keypress (WASD)
        // rb.velocity.y deals with falling + jumping by setting velocity to y. 
        rb.velocity = new Vector2(moveInput * speed, moveInput * rb.velocity.y);
    }
}
