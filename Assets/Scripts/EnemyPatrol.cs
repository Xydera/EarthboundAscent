using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.collider.GetComponent<Player>())
        {

            collision.collider.GetComponent<Player>().Damage();

        }

    }
}
