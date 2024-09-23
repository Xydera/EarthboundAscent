using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    bool CanRestart = false;
    public void Setup()
    {
        transform.localScale = new Vector2(10,10);
        gameObject.SetActive(true);
        StartCoroutine(RestartDelay());
        transform.LeanScale(Vector2.one, 1f).setEaseOutExpo().delay = 2;
    }

    private IEnumerator RestartDelay()
    {
        CanRestart = false;
        yield return new WaitForSeconds(4F);
        CanRestart = true;

    }

    void Update()
    {
        if (Input.anyKey && CanRestart)
        {
            SceneManager.LoadScene("LevelOne");
        }        
    }

}
