using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomSlider : MonoBehaviour
{
    [Range(0f, 1f), SerializeField] private float _value;
    [SerializeField] private Image slidingImage;

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

    private void OnValidate()
    {
        slidingImage.fillAmount = _value;
    }
}
