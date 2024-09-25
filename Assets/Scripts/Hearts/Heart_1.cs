using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart_1 : MonoBehaviour
{
    public Sprite Damage;
    public void Healthdown()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = Damage;
    }
}
