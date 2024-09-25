using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;                  // Amount of damage the bullet does
    public float speed;                 // Speed of the bullet
    public float deathDistance;         // Distance after which the bullet is destroyed

    void Update()
    {
        // Move the bullet upward
        transform.Translate(Vector2.up * Time.deltaTime * speed);

        // Destroy the bullet if it travels beyond the death distance
        if (Vector2.Distance(transform.position, Vector2.zero) > deathDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collided object has the tag "Enemy"
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Get the EnemyHealth component from the collided enemy
            EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();

            // Ensure the enemyHealth component is found before trying to call TakeDamage
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);  // Call the TakeDamage method with damage value
                Debug.Log("Collided with enemy");
            }
            else
            {
                Debug.LogWarning("Enemy object has no EnemyHealth component!");
            }

            // Destroy the bullet after the collision
            
        }
        Destroy(gameObject);
    }
}
