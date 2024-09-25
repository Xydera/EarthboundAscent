using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 2;
    public int health;
    private bool Dead = false;

    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        anim = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        if (!Dead)
        {
            if (health > 0)
            {
                anim.SetTrigger("Hurt");
                health -= damage;
                if (health <= 0)
                {
                    anim.SetTrigger("Death");
                    Dead = true;
                }
            }
        }

    }
}
