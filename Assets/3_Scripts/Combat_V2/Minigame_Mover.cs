using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigame_Mover : MonoBehaviour
{
    private Dictionary<Transform, Coroutine> routines = new();

    public void MoveLocal(Transform transform, Vector3 to, float time, Action onComplete = null)
    {
        if (routines.ContainsKey(transform))
            StopCoroutine(routines[transform]);

        Coroutine coroutine = StartCoroutine(MoveLocalTransform());

        if (routines.ContainsKey(transform))
            routines[transform] = coroutine;
        else
            routines.Add(transform, coroutine);

        IEnumerator MoveLocalTransform()
        {
            float timer = 0f;

            Vector3 pos = transform.localPosition;

            while (timer < time)
            {
                timer += Time.deltaTime;
                //float easedT = EasingFunctions.EaseOutExpo(timer / time);
                
                float easedT = timer / time;
                transform.localPosition = Vector3.Lerp(pos, to, easedT);
                yield return null;
            }

            onComplete?.Invoke();
        }
    }

    public void Cancel(Transform transform)
    {
        if (routines.ContainsKey(transform))
            StopCoroutine(routines[transform]);
    }
}
