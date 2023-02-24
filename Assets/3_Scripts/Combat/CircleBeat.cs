using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleBeat : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] Image innerImg;
    [SerializeField] Image outerImg;
    [SerializeField] Image inputImage;

    [Header("Settings")]
    public KeyInput key;
    public int onBeatCount = 4;
    [SerializeField] private float bufferMargin = 0.3f;

    public bool startTrace;
    public bool end { get; set; }

    private bool start; 
    private float timer, timeToBeatCount;

    public RectTransform rect { get; set; }

    public CircleBeat nextBeat { get; set; }
    public Action<CircleBeat> failCallback, successCallback;

    private void OnEnable()
    {
        rect = GetComponent<RectTransform>();

        TempoManager.OnBeat += TempoManager_OnBeat;

        innerImg.color = new Color(innerImg.color.r, innerImg.color.g, innerImg.color.b, 0);
        outerImg.color = new Color(0, 1, 0, 0);
        inputImage.color = new Color(1, 1, 1, 0);

        LeanTween.color(innerImg.rectTransform, new Color(innerImg.color.r, innerImg.color.g, innerImg.color.b, 1), 1f);
        LeanTween.color(outerImg.rectTransform, new Color(0, 1, 0, 1), 1f);
        LeanTween.color(inputImage.rectTransform, new Color(1, 1, 1, 1), 1f);
    }

    private void OnDisable()
    {
        TempoManager.OnBeat -= TempoManager_OnBeat;
    }

    private void TempoManager_OnBeat()
    {
        if (!start) start = true;
    }

    void Start()
    {
        switch (key)
        {
            case KeyInput.Circle:
                inputImage.sprite = SpriteData.Instance.circle;
                break;
            case KeyInput.Cross:
                inputImage.sprite = SpriteData.Instance.cross;
                break;
            case KeyInput.Square:
                inputImage.sprite = SpriteData.Instance.square;
                break;
            case KeyInput.Triangle:
                inputImage.sprite = SpriteData.Instance.triangle;
                break;
        }

        timeToBeatCount = TempoManager.GetTimeToBeatCount(onBeatCount);
    }

    void Update()
    {
        if (end) enabled = false;
        if (!start) return;

        timer += Time.deltaTime;

        if (startTrace)
        {
            if (InputReceiver.Instance.AnyActionKey() && InputReceiver.IsWrongInput(key))
            {
                InputReceiver.ToggleOffAllInput();

                outerImg.color = Color.red;
                end = true;

                failCallback?.Invoke(this);
            }

            if (InputReceiver.ReceiveInput(key))
            {
                Debug.Log("YO");
                InputReceiver.ToggleOffInput(key);

                if (timer > timeToBeatCount + bufferMargin)
                {
                    outerImg.color = Color.red;
                    end = true;

                    failCallback?.Invoke(this);
                }
                else if (timer >= timeToBeatCount - bufferMargin && timer <= timeToBeatCount + bufferMargin)
                {
                    outerImg.color = Color.blue;
                    outerImg.rectTransform.sizeDelta = innerImg.rectTransform.sizeDelta;
                    end = true;

                    if (nextBeat != null)
                        nextBeat.startTrace = true;

                    successCallback?.Invoke(this);
                }
                else if (timer < timeToBeatCount - bufferMargin)
                {
                    outerImg.color = Color.yellow;
                    end = true;

                    failCallback?.Invoke(this);
                }
            }

            if (timer > timeToBeatCount + bufferMargin)
            {
                outerImg.color = Color.red;
                end = true;

                failCallback?.Invoke(this);
            }
        }

        if (!end)
        {
            const float lerpSpeed = 1.0f;
            Vector2 outerOri = innerImg.rectTransform.sizeDelta * (1.0f + lerpSpeed * timeToBeatCount);

            float minFactor = 0.5f;
            Vector2 outerMin = innerImg.rectTransform.sizeDelta * minFactor;

            if (timer <= timeToBeatCount)
            {
                float lerpProgress = timer / timeToBeatCount;

                Vector2 targetSize = Vector2.Lerp(outerOri, innerImg.rectTransform.sizeDelta, lerpProgress);
                targetSize.x = Mathf.Max(targetSize.x, outerMin.x);
                targetSize.y = Mathf.Max(targetSize.y, outerMin.y);

                outerImg.rectTransform.sizeDelta = targetSize;
            }
            else
            {
                float lerpProgress = (timer - timeToBeatCount) / (timeToBeatCount * minFactor);

                Vector2 targetSize = Vector2.Lerp(innerImg.rectTransform.sizeDelta, outerMin, lerpProgress);
                targetSize.x = Mathf.Max(targetSize.x, outerMin.x);
                targetSize.y = Mathf.Max(targetSize.y, outerMin.y);

                outerImg.rectTransform.sizeDelta = targetSize;
            }
        }
    }
}
