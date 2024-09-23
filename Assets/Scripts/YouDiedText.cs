using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class YouDiedText : MonoBehaviour
{
    private void Start()
    {
        transform.LeanScale(Vector2.one, 1f).setEaseOutExpo(); ;
    }

}
