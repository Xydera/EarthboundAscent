using UnityEngine;

public class Player : MonoBehaviour
{

    public float speed;
    public float jump;
    public float groundedY;
    public int maxJumps;

    new Rigidbody2D rb;

    int jumpsLeft;
    bool controls = true;
    Vector2 startPosition;
    Animator animator;

    private void Awake()
    {
        startPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }


    void Update()
    {
        if (controls)
        {
            // Move
            transform.Translate(Vector2.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime);

            // Jump
            CheckJump();
            CheckAnimations();
        }


    }

    public bool IsGrounded()
    {

        RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0, groundedY), Vector2.down, .1f);

        if (hit.collider != null)
        {

            return true;

        }

        return false;

    }

    void CheckJump()
    {

        if (IsGrounded() && rb.velocity.y == 0) { jumpsLeft = maxJumps; }

        if (Input.GetButtonDown("Jump") && jumpsLeft > 0)
        {

            rb.velocity = new Vector2(rb.velocity.x, 0);

            GetComponent<Rigidbody2D>().AddForce(Vector2.up * jump, ForceMode2D.Impulse);

            animator.Play("jump");

            if (Input.GetAxis("Horizontal") < 0) { animator.Play("jumpLeft"); }

            jumpsLeft--;

        }
    }
        private void OnDrawGizmos()
    {

        Gizmos.color = Color.red;

        Gizmos.DrawRay(transform.position + new Vector3(0, groundedY), Vector2.down * .1f);

    }

    public void Die()
    {
        transform.position = startPosition;
    }

    public bool IsJumpFinished()
    {

        if (!IsGrounded()) { return false; }

        if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("Jump")) { return true; }

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < animator.GetCurrentAnimatorStateInfo(0).length) { return false; }

        return true;
    }

    void CheckAnimations()
    {
        if (IsJumpFinished())
        {
            if (Input.GetAxis("Horizontal") < 0)
            {

                animator.Play("runLeft");

            }
            else if (Input.GetAxis("Horizontal") > 0)
            {

                animator.Play("runRight");

            }
            else
            {

                animator.Play("Idle");

            }
        }
            
    }

}