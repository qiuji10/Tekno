using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatFill : MonoBehaviour
{
    [SerializeField] int totalBeatCount = 4;
    [SerializeField] Image image;
    [SerializeField] Image cir, targetCir;

    private int _beatCount = 0;

    private void OnEnable()
    {
        TempoManager.OnBeat += OnBeat;
    }

    private void OnDisable()
    {
        TempoManager.OnBeat -= OnBeat;
    }

    private void OnBeat()
    {

        if (_beatCount == 0)
        {
            float timeToBeat = TempoManager.GetTimeToBeatCount(totalBeatCount);
            StartCoroutine(RectLerper(timeToBeat));
            //StartCoroutine(CirLerper(cir.rectTransform, targetCir.rectTransform.sizeDelta, timeToBeat));
        }

        _beatCount++;

        if (_beatCount >= totalBeatCount)
        {
            _beatCount = 0;
        }
    }

    private IEnumerator RectLerper(float timeToBeat)
    {
        float timer = 0;

        while (timer <= timeToBeat)
        {
            timer += Time.deltaTime;
            image.fillAmount = timer / timeToBeat;
            yield return null;
        }

        image.fillAmount = 0;
    }

    //private IEnumerator CirLerper(RectTransform rectTransform, Vector2 endSize, float timeToBeat)
    //{
    //    float timer = 0;
    //    Vector2 startSize = rectTransform.sizeDelta;

    //    while (timer <= timeToBeat)
    //    {
    //        timer += Time.deltaTime;

    //        // Calculate the new size of the image using Lerp
    //        Vector2 size = Vector2.Lerp(startSize, endSize, timer / timeToBeat);

    //        // Set the size of the image
    //        rectTransform.sizeDelta = size;

    //        yield return null;
    //    }

    //    // Set the size of the image to the final size
    //    rectTransform.sizeDelta = startSize;
    //}
}
