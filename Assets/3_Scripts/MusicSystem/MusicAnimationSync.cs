using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

public class MusicAnimationSync : MonoBehaviour
{
    public Animator animator;

    [EventID]
    public string eventID;

    // Start is called before the first frame update
    void Awake()
    {
        Koreographer.Instance.RegisterForEvents(eventID, OnAnimationTrigger);
        animator = GetComponent<Animator>();
    }

    void OnAnimationTrigger(KoreographyEvent evt)
    {
        float speedValue = evt.GetFloatValue();
        animator.speed = speedValue;
        Debug.Log("animation speed it " + animator.speed);
        animator.Play("Bboy Hip Hop Move", 0);
    }
}
