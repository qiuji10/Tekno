using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeCanvas : MonoBehaviour
{
    public static FadeCanvas Instance;

    private int fadeIn, fadeOut;
    private Animator _anim;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);

        _anim = GetComponent<Animator>();
        fadeIn = Animator.StringToHash("FadeIn");
        fadeOut = Animator.StringToHash("FadeOut");
    }

    [Button]
    public void FadeIn()
    {
        _anim.SetTrigger(fadeIn);
    }

    [Button]
    public void FadeOut()
    {
        _anim.SetTrigger(fadeOut);
    }
}
