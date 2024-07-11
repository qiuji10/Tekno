using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatPoint : MonoBehaviour
{
    public Image img { get; set; }
    public RectTransform rect { get; set; }

    //private Slider slider;
    private CustomSlider mainSlider, bgSlider;
    private RectTransform rotator, sliderRect, bgSliderRect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        img = transform.GetChild(2).GetComponent<Image>();
        rotator = transform.GetChild(0).GetComponent<RectTransform>();
        //slider = GetComponentInChildren<Slider>();

        mainSlider = rotator.GetChild(0).GetComponent<CustomSlider>();
        bgSlider = rotator.GetChild(1).GetComponent<CustomSlider>();

        sliderRect = mainSlider.GetComponent<RectTransform>();
        bgSliderRect = bgSlider.GetComponent<RectTransform>();
    }

    public void Scale(int index, Transform sliderParent, float scalePos, Direction dir, float scale, float timeReachEndPos)
    {
        rotator.transform.SetParent(sliderParent);
        rotator.anchoredPosition = rect.anchoredPosition;

        switch (dir)
        {
            case Direction.Up:
                rotator.eulerAngles = new (0, 0, 90);
                break;
            case Direction.Down:
                rotator.eulerAngles = new(0, 0, 270);
                break;
            case Direction.Left:
                rotator.eulerAngles = new(0, 0, 180);
                break;
            case Direction.Right:
                rotator.eulerAngles = new(0, 0, 0);
                break;
        }

        sliderRect.sizeDelta = bgSliderRect.sizeDelta = new Vector2(scale, sliderRect.sizeDelta.y);
        sliderRect.anchoredPosition = bgSliderRect.anchoredPosition = new Vector2(scalePos, 0);
        LeanTween.value(0, 1, timeReachEndPos).setOnUpdate((float value) =>
        { 
            if (index == 1)
            {
                mainSlider.Value = value;
            }
            else if (index == 2)
            {
                bgSlider.Value = value;
            }
        }).setEaseOutExpo();
    }
}
