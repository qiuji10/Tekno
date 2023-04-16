using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveController : MonoBehaviour
{
    public Material material;
    public float dissolveAmount = 0f;
    public float dissolveSpeed = 1f;

    void Update()
    {
        material.SetFloat("_Dissolve", dissolveAmount);
    }

    public void SetDissolveAmount(float endValue, float duration)
    {
        StartCoroutine(DissolveCoroutine(endValue, duration));
    }

    IEnumerator DissolveCoroutine(float endValue, float duration)
    {
        float startValue = dissolveAmount;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            dissolveAmount = Mathf.Lerp(startValue, endValue, timeElapsed / duration);

            yield return null;
        }

        dissolveAmount = endValue;
    }
}
