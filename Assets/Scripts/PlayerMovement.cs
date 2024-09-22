using UnityEngine;

public class Player : MonoBehaviour
{

    public float speed;
    public float jump;
    public float groundedY;
    public int maxJumps;
    public float health;
    public Shoot shoot;
    public Animations animations;

    new Rigidbody2D rb;

    int jumpsLeft;
    bool controls = true;
    bool dead;
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
            CheckShoot();
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
    void CheckShoot()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            
            // Instantiate the bullet
            GameObject bullet = Instantiate(shoot.prefab.gameObject);

            // Set the bullet's initial position to the player's position
            bullet.transform.position = transform.position;

            // Get the mouse position in world space
            Vector3 mousePositionRaw = Input.mousePosition;
            mousePositionRaw.z = 10; // Set a proper distance for 2D projection

            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(mousePositionRaw);

            // Calculate the direction from the bullet to the mouse position
            Vector3 direction = mousePosition - bullet.transform.position;

            // Calculate the angle in 2D
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Set the bullet's rotation
            bullet.transform.rotation = Quaternion.Euler(0, 0, angle);

            // Move the bullet forward
            bullet.transform.Translate(Vector2.up * 1.5f);
        }
    }


    void CheckJump()
    {

        if (IsGrounded() && rb.velocity.y == 0) { jumpsLeft = maxJumps; }

        if (Input.GetButtonDown("Jump") && jumpsLeft > 0)
        {

            rb.velocity = new Vector2(rb.velocity.x, 0);

            GetComponent<Rigidbody2D>().AddForce(Vector2.up * jump, ForceMode2D.Impulse);

            if (Input.GetAxis("Horizontal") < 0)
            {

                animator.Play("jumpLeft");

            }
            else if (Input.GetAxis("Horizontal") > 0)
            {

                animator.Play("jumpRight");

            }

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

        animator.Play(animations.death.name, 0, 0);
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

[System.Serializable]
public struct Animations
{

    public AnimationClip runLeft;
    public AnimationClip runRight;
    public AnimationClip jumpLeft;
    public AnimationClip jumpRight;
    public AnimationClip death;

}

[System.Serializable]
public struct Shoot
{

    public Bullet prefab;

}