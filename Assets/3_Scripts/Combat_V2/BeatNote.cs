using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatNote : MonoBehaviour
{
    [SerializeField] Image image;

    public void SetPosition(Vector2 anchoredPosition)
    {
        (transform as RectTransform).anchoredPosition = anchoredPosition;
    }

    public void SetSprite(Sprite sprite)
    {
        image.sprite = sprite;
    }

    public void SetColor(Color color)
    {
        image.color = color;
    }
}
