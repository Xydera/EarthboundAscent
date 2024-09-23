using System.Collections;
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
    private bool CanStab = true;
    private bool CanDamage = true;
    private bool Flipped = false;
    private bool SoundRun = true;

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
        deathSpawn.transform.position = transform.position;
        if (health == 0)
        {
            Die();
        }

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
        if (Input.GetButtonDown("Fire2"))
        {
            if (CanShoot)
            {
                // Play attack sound FX
                SoundFXManager.instance.PlayRandomSoundFXClip(AttackSoundClips, transform, 0.2f);
                animator.Play("throwStar");
                
                // Instantiate the bullet
                GameObject bullet = Instantiate(shoot.prefab.gameObject);

                // Set the bullet's initial position to the player's position
                bullet.transform.position = transform.position;

                // Get the mouse position in world space
                Vector3 mousePositionRaw = Input.mousePosition;
                mousePositionRaw.z = 1; // Set a proper distance for 2D projection

                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(mousePositionRaw);

                // Calculate the direction from the bullet to the mouse position
                Vector3 direction = mousePosition - bullet.transform.position;

                // Calculate the angle in 2D
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                // Set the bullet's rotation
                bullet.transform.rotation = Quaternion.Euler(0, 0, angle-90);

                // Move the bullet forward
                bullet.transform.Translate(Vector2.up * 1.5f);

                ammo--;
                StartCoroutine(ShootDelay());
                if (ammo == 0)
                {
                    StartCoroutine(ReloadDelay());
                }
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
        CanShoot = false;
        yield return new WaitForSeconds(0.25F);
        if (ammo > 0)
        {
            CanShoot = true;
        }

    }

    private IEnumerator ReloadDelay()
    {
        CanShoot = false;
        yield return new WaitForSeconds(1);
        CanShoot = true;
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

        if (IsGrounded() && rb.velocity.y == 0) { jumpsLeft = maxJumps; }

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

        if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("Jump")) { return true; }

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < animator.GetCurrentAnimatorStateInfo(0).length) { return false; }

        return true;
    }

    void CheckAnimations()
    {
        if (IsJumpFinished())
        {
            if (CanShoot)
            {
                    if (Input.GetAxis("Horizontal") < 0)
                {
                    if (SoundRun)
                    {
                        SoundRun = false;
                        StartCoroutine(RunSoundDelay());
                        SoundFXManager.instance.PlayRandomSoundFXClip(RunSoundClips, transform, 0.05f);
                    }
                    
                    animator.Play("runLeft");

                }
                else if (Input.GetAxis("Horizontal") > 0)
                {
                    if (SoundRun)
                    {
                        SoundRun = false;
                        StartCoroutine(RunSoundDelay());
                        SoundFXManager.instance.PlayRandomSoundFXClip(RunSoundClips, transform, 0.05f);
                    }
                       
                    animator.Play("runRight");

                }
                else
                {                
                        animator.Play("Idle");
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