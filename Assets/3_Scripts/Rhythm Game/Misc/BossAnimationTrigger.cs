using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum Hand { Left, Right }

public class BossAnimationTrigger : MonoBehaviour
{
    [SerializeField] private Hand hand;
    [SerializeField] private bool isCharge;

    [Header("References")]
    [SerializeField] private Animator anim;

    private const int LeftLayer = 1;
    private const int RightLayer = 2;
    private const float TapLeft = 0, TapBoth = 0.5f, TapRight = 1;

    private int BlendTap, Tap, ChargeLeft, ChargeRight, HoldLeft, HoldRight;

    private void Awake()
    {
        BlendTap = Animator.StringToHash("BlendTap");
        Tap = Animator.StringToHash("Tap");
        ChargeLeft = Animator.StringToHash("ChargeLeft");
        ChargeRight = Animator.StringToHash("ChargeRight");
        HoldLeft = Animator.StringToHash("HoldLeft");
        HoldRight = Animator.StringToHash("HoldRight");
    }

    private void OnTriggerEnter(Collider col)
    {
        
    }

    [Button]
    private void TapLeft_Animation()
    {
        anim.SetLayerWeight(LeftLayer, 0);
        anim.SetFloat(BlendTap, TapLeft);
        anim.SetTrigger(Tap);
    }

    [Button]
    private void TapRight_Animation()
    {
        anim.SetLayerWeight(RightLayer, 0);
        anim.SetFloat(BlendTap, TapRight);
        anim.SetTrigger(Tap);
    }

    [Button]
    private void TapBoth_Animation()
    {
        anim.SetLayerWeight(LeftLayer, 0);
        anim.SetLayerWeight(RightLayer, 0);
        anim.SetFloat(BlendTap, TapBoth);
        anim.SetTrigger(Tap);
    }
}
