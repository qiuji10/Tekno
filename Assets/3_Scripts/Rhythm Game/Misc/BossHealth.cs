using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [SerializeField] private Animator anim;

    public void PlayHit()
    {
        anim.SetTrigger("Hit");
    }
}
