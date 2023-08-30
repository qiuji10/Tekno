using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatLeftRIGHT : MonoBehaviour
{
    RectTransform rect;

    [SerializeField] private LeanTweenType type;
    private Vector2 left = new Vector2 (-500, 0);
    private Vector2 right = new Vector2 (500, 0);

    bool isRight = true;

    private void Awake()
    {
        rect = transform as RectTransform;
    }

    private void OnEnable()
    {
        TempoManager.OnBeat += TempoManager_OnBeat;
    }

    private void OnDisable()
    {
        TempoManager.OnBeat -= TempoManager_OnBeat;
    }

    private void TempoManager_OnBeat()
    {
        if (isRight)
        {
            LeanTween.value(0, 1, 60f / 140f).setOnUpdate((float value) =>
            {
                rect.anchoredPosition = Vector2.Lerp(left, right, value);
            }).setEase(type);
        }
        else
        {
            LeanTween.value(0, 1, 60f / 140f).setOnUpdate((float value) => {
                rect.anchoredPosition = Vector2.Lerp(right, left, value);
            }).setEase(type);
        }

        isRight = !isRight;
    }
}
