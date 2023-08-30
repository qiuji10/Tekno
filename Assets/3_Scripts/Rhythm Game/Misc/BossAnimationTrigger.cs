using NaughtyAttributes;
using System.Collections;
using UnityEngine;

public enum Hand { Left, Right }
public enum BossAction { Tap, Charge, Hold }

public class BossAnimationTrigger : MonoBehaviour
{
    [SerializeField] private Hand hand;
    [SerializeField] private BossAction action;

    [Header("References")]
    [SerializeField] private Animator anim;

    private const int LeftHoldLayer = 1;
    private const int RightHoldLayer = 2;
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

        NoteObject_Hold.OnFullySurpassStart += StopHoldAnimation;
    }

    private void OnDestroy()
    {
        NoteObject_Hold.OnFullySurpassStart -= StopHoldAnimation;
    }

    private void StopHoldAnimation(Lane lane)
    {
        switch (lane)
        {
            case Lane.Lane1:
            case Lane.Lane3:
                HoldRight_Stop_Animation();
                break;
            case Lane.Lane2:
            case Lane.Lane4:
                HoldLeft_Stop_Animation();
                break;
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        switch (action)
        {
            case BossAction.Tap:
                if (col.CompareTag("Note"))
                {
                    if (hand == Hand.Left)
                    {
                        TapLeft_Animation();
                    }
                    else if (hand == Hand.Right)
                    {
                        TapRight_Animation();
                    }
                }

                break;
            case BossAction.Charge:

                if (col.CompareTag("LongNote"))
                {

                    if (hand == Hand.Left)
                    {
                        ChargeLeft_Animation();
                    }
                    else if (hand == Hand.Right)
                    {
                        ChargeRight_Animation();
                    }
                }

                break;
            case BossAction.Hold:

                if (col.CompareTag("LongNote"))
                {

                    if (hand == Hand.Left)
                    {
                        HoldLeft_Animation();
                    }
                    else if (hand == Hand.Right)
                    {
                        HoldRight_Animation();
                    }
                }
                break;
        };
    }

    [Button]
    private void TapLeft_Animation()
    {
        anim.SetLayerWeight(LeftHoldLayer, 0);
        anim.SetFloat(BlendTap, TapLeft);
        anim.SetTrigger(Tap);
    }

    [Button]
    private void TapRight_Animation()
    {
        anim.SetLayerWeight(RightHoldLayer, 0);
        anim.SetFloat(BlendTap, TapRight);
        anim.SetTrigger(Tap);
    }

    [Button]
    private void TapBoth_Animation()
    {
        anim.SetLayerWeight(LeftHoldLayer, 0);
        anim.SetLayerWeight(RightHoldLayer, 0);
        anim.SetFloat(BlendTap, TapBoth);
        anim.SetTrigger(Tap);
    }

    [Button]
    private void ChargeLeft_Animation()
    {
        anim.SetLayerWeight(LeftHoldLayer, 1);
        anim.SetTrigger(ChargeLeft);
    }

    [Button]
    private void ChargeRight_Animation()
    {
        anim.SetLayerWeight(RightHoldLayer, 1);
        anim.SetTrigger(ChargeRight);
    }

    private void HoldLeft_Animation()
    {
        anim.SetLayerWeight(LeftHoldLayer, 1);
        anim.SetTrigger(HoldLeft);
    }

    private void HoldRight_Animation()
    {
        anim.SetLayerWeight(RightHoldLayer, 1);
        anim.SetTrigger(HoldRight);
    }

    private void HoldLeft_Stop_Animation()
    {
        anim.SetLayerWeight(LeftHoldLayer, 0);
        SetLayerSmooth(LeftHoldLayer, 0.5f);
    }

    private void HoldRight_Stop_Animation()
    {
        anim.SetLayerWeight(RightHoldLayer, 0);
        SetLayerSmooth(RightHoldLayer, 0.5f);
    }

    private void SetLayerSmooth(int layer, float duration)
    {
        float from = 0, to = 0;

        switch (layer)
        {
            case 1: from = 0; to = 1; break;
            case 2: from = 1; to = 0; break;
        }

        StartCoroutine(SetLayerSmooth_Task(layer, from, to, duration));
    }

    private IEnumerator SetLayerSmooth_Task(int layer, float from, float to, float duration)
    {
        float timer = 0;
        
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float ratio = timer / duration;

            anim.SetLayerWeight(layer, Mathf.Lerp(from, to, ratio));

            yield return null;
        }

        anim.SetLayerWeight(layer, to);
    }
}
