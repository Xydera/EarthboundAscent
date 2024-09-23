using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathSpawn : MonoBehaviour
{
    [SerializeField] private AudioClip DeathSoundClip;



    public Animations animations;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        SoundFXManager.instance.PlaySoundFXClip(DeathSoundClip, transform, 0.2f);
        animator.Play("death");
    }
    [System.Serializable]
    public struct Animations
    {
        public AnimationClip death;

    }
}
