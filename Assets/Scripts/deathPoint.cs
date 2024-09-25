using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deathPoint : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.collider.GetComponent<Player>())
        {

            collision.collider.GetComponent<Player>().Die();

        }

    }
}
