using UnityEngine;

public class Bullet : MonoBehaviour
{

    public float speed;
    public float deathDistance;

    void Update()
    {

        transform.Translate(Vector2.up * Time.deltaTime * speed);

        if (Vector2.Distance(transform.position, Vector2.zero) > deathDistance)
        {

            Destroy(gameObject);

        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {



        Destroy(gameObject);

    }

}