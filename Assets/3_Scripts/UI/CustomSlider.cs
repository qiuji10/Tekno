using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomSlider : MonoBehaviour
{
    [Range(0f, 1f), SerializeField] private float _value;
    [SerializeField] private Image slidingImage;

    private Coroutine lerping;

    public float Value
    {
        get { return _value; }
        set
        {
            _value = value;

            if (slidingImage)
                slidingImage.fillAmount = _value;
        }
    }

    public void Lerp(float from, float to, float time, Action onComplete = null)
    {
        if (lerping != null)
            StopCoroutine(lerping);

        lerping = StartCoroutine(Lerp());

        IEnumerator Lerp()
        {
            float timer = 0f;

            while (timer < time)
            {
                timer += Time.deltaTime;
                //float easedT = EasingFunctions.EaseOutExpo(Mathf.Clamp01(timer / time));
                float easedT = timer / time;
                Value = Mathf.Lerp(from, to, easedT);
                yield return null;
            }

            onComplete?.Invoke();
        }
    }

    public void StopLerp()
    {
        if (lerping != null)
            StopCoroutine(lerping);
    }

    private void OnValidate()
    {
        slidingImage.fillAmount = _value;
    }
}
