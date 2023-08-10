using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Hand { Left, Right }

public class BossAnimationTrigger : MonoBehaviour
{
    [SerializeField] private Hand hand;
    [SerializeField] private bool isCharge;

    [Header("References")]
    [SerializeField] private Animator anim;

    private const int leftLayer = 1;
    private const int rightLayer = 2;

    private void OnTriggerEnter(Collider col)
    {
        
    }
}
