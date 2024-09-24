using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private AudioClip[] SingleJumpSoundClips;
    [SerializeField] private AudioClip[] AttackSoundClips;
    [SerializeField] private AudioClip[] DamagedSoundClips;
    [SerializeField] private AudioClip[] RunSoundClips;

    [SerializeField] private AudioClip DoubleJumpSoundClip;


    public GameObject deathSpawn;
    public GameObject MainCamera;

    public GameOverScreen GameOverScreen;



    public float speed;
    public float jump;
    public float groundedY;
    public int maxJumps;
    public float health;
    public float ammo;
    public Shoot shoot;
    public Animations animations;

    private bool CanShoot = true;
    private bool Reloading = false;
    private bool CanStab = true;
    private bool CanDamage = true;
    private bool SoundRun = true;
    private bool isThrowing = false; // Track if player is in the middle of the throw animation

    Rigidbody2D rb;

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
        deathSpawn.transform.position = transform.position;
        if (health == 0)
        {
            Die();
        }

        if (controls)
        {
            // Movement
            float horizontal = Input.GetAxis("Horizontal");
            transform.Translate(Vector2.right * horizontal * speed * Time.deltaTime);

            // Handle sprite flipping only if NOT throwing
            if (!isThrowing)
            {
                // Normal flipping logic while grounded or in mid-air
                if (!IsGrounded() || horizontal != 0)
                {
                    // Flip based on movement direction
                    if (horizontal < 0)
                    {
                        transform.localScale = new Vector3(-1, 1, 1); // Face left
                    }
                    else if (horizontal > 0)
                    {
                        transform.localScale = new Vector3(1, 1, 1); // Face right
                    }
                }
            }
            else
            {
                // While throwing, the flip is controlled by the cursor position (override)
                FlipBasedOnCursor();
            }

            // Handle the rest of the player actions
            CheckJump();
            CheckAnimations();
            CheckShoot();
            CheckStab();
        }
    }

    void FlipBasedOnCursor()
    {
        // Get the mouse position in world space
        Vector3 mousePositionRaw = Input.mousePosition;
        mousePositionRaw.z = 1; // Set a proper distance for 2D projection
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(mousePositionRaw);

        // Flip sprite based on mouse position relative to the player
        if (mousePosition.x < transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1); // Face left
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1); // Face right
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
        if (Input.GetButtonDown("Fire2") && CanShoot && !isThrowing && !Reloading)
        {
            // Set the trigger to play the throwStar animation
            animator.SetTrigger("ThrowStar");

            // Play attack sound FX
            SoundFXManager.instance.PlayRandomSoundFXClip(AttackSoundClips, transform, 0.2f);

            // Instantiate the bullet
            GameObject bullet = Instantiate(shoot.prefab.gameObject);
            bullet.transform.position = transform.position;

            Vector3 mousePositionRaw = Input.mousePosition;
            mousePositionRaw.z = 1; // Set a proper distance for 2D projection
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(mousePositionRaw);
            Vector3 direction = mousePosition - bullet.transform.position;

            // Calculate the angle in 2D and set the bullet's rotation
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.Euler(0, 0, angle - 90);

            // Move the bullet forward
            bullet.transform.Translate(Vector2.up * 1.5f);

            // Lock the player in the throw animation until it's finished
            StartCoroutine(ShootDelay());

            ammo--;
            if (ammo == 0)
            {
                StartCoroutine(ReloadDelay());
            }
        }
    }

    void CheckStab()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (CanStab)
            {

                SoundFXManager.instance.PlayRandomSoundFXClip(AttackSoundClips, transform, 0.2f);
                animator.Play("attack");


            }
        }
    }

    private IEnumerator StabDelay()
    {
        CanStab = false;
        yield return new WaitForSeconds(0.7F);
        CanStab = true;
    }

    private IEnumerator ShootDelay()
    {
        // Wait until the throwStar animation is done
        if (!Reloading){
            CanShoot = false;
            isThrowing = true;
            FlipBasedOnCursor();
            yield return new WaitForSeconds((animator.GetCurrentAnimatorStateInfo(0).length) / 12); // Wait until animation finishes

            // Allow other animations after throwing is done
            isThrowing = false;
            CanShoot = true;
        }
        
    }

    private IEnumerator ReloadDelay()
    {
        Reloading = true;
        yield return new WaitForSeconds(1);
        Reloading = false;
        ammo = 3;
    }

    private IEnumerator DamageDelay()
    {
        CanDamage = false;
        yield return new WaitForSeconds(0.5F);
        CanDamage = true;
    }

    private IEnumerator RunSoundDelay()
    {
        yield return new WaitForSeconds(0.3F);
        SoundRun = true;


    }

    void CheckJump()
    {
        if (IsGrounded() && rb.velocity.y == 0)
        {
            jumpsLeft = maxJumps;
        }

        if (Input.GetButtonDown("Jump") && jumpsLeft > 0)
        {
            if (jumpsLeft == 1)
            {
                SoundFXManager.instance.PlaySoundFXClip(DoubleJumpSoundClip, transform, 0.2f);
            }
            else
            {
                SoundFXManager.instance.PlayRandomSoundFXClip(SingleJumpSoundClips, transform, 0.2f);
            }

            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * jump, ForceMode2D.Impulse);

            // Flip the sprite based on horizontal movement during the jump
            if (Input.GetAxis("Horizontal") < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (Input.GetAxis("Horizontal") > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }

            // Use the "jumpRight" animation, flip will handle direction
            if (!isThrowing)
            {
                animator.Play("jumpRight");
            }
            
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
        MainCamera.transform.parent = null;
        Instantiate(deathSpawn);
        deathSpawn.transform.position = transform.position;

        GameOverScreen.Setup();

        Destroy(gameObject);

    }

    public void Damage()
    {
        if (CanDamage)
        {
            if (health > 0)
            {
                SoundFXManager.instance.PlayRandomSoundFXClip(DamagedSoundClips, transform, 0.2f);
                health--;
            }


            StartCoroutine(DamageDelay());
        }
    }

    public bool IsJumpFinished()
    {

        if (!IsGrounded()) { return false; }

        if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("JumpRight")) { return true; }

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < animator.GetCurrentAnimatorStateInfo(0).length) { return false; }

        return true;
    }

    void CheckAnimations()
    {
        if (isThrowing)
        {
            // Prevent other animations from playing while throwing
            return;
        }

        if (IsJumpFinished())
        {
            if (CanShoot)
            {
                float horizontal = Input.GetAxis("Horizontal");

                if (horizontal < 0)
                {
                    // Flip the sprite to face left
                    transform.localScale = new Vector3(-1, 1, 1);
                    if (SoundRun)
                    {
                        SoundRun = false;
                        StartCoroutine(RunSoundDelay());
                        SoundFXManager.instance.PlayRandomSoundFXClip(RunSoundClips, transform, 0.05f);
                    }
                    if (!isThrowing)
                    {
                        animator.Play("runRight");
                    }
                    
                }
                else if (horizontal > 0)
                {
                    // Flip the sprite to face right
                    transform.localScale = new Vector3(1, 1, 1);
                    if (SoundRun)
                    {
                        SoundRun = false;
                        StartCoroutine(RunSoundDelay());
                        SoundFXManager.instance.PlayRandomSoundFXClip(RunSoundClips, transform, 0.05f);
                    }
                    if (!isThrowing)
                    {
                        animator.Play("runRight");
                    }
                }
                else
                {
                    if (!isThrowing)
                    {
                        animator.Play("Idle");
                    }
                    
                }
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
    public AnimationClip throwStar;
    public AnimationClip attack;

}

[System.Serializable]
public struct Shoot
{

    public Bullet prefab;

}