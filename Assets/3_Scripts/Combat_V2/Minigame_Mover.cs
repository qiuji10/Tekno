using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigame_Mover : MonoBehaviour
{
    public void MoveLocal(Transform transform, Vector3 to, float time, Action onComplete = null)
    {
        StartCoroutine(MoveLocalTransform());

        IEnumerator MoveLocalTransform()
        {
            float timer = 0f;

            while (timer < time)
            {
                timer += Time.deltaTime;
                float easedT = EasingFunctions.EaseOutExpo(timer / time);
                transform.localPosition = Vector3.Lerp(transform.localPosition, to, easedT);
                yield return null;
            }

            onComplete?.Invoke();
        }
    }
}
