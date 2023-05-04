using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectrumElement_UI : MonoBehaviour
{
    public RectTransform rect { get; set; }
    
    private float _peakScale;
    private float _currentScale;
    private float _lastUpdate;

    private SpectrumUI spectrum;
    private Vector3 scale;

    private void Awake()
    {
        spectrum = FindObjectOfType<SpectrumUI>();
        rect = GetComponent<RectTransform>();
        scale = rect.localScale;
    }

    public void SetScale(float scale)
    {
        if (scale > _currentScale)
        {
            _peakScale = _currentScale = scale;
            _lastUpdate = Time.unscaledTime;
        }
    }

    private void Update()
    {
        var t = (Time.unscaledTime - _lastUpdate) / spectrum.releaseTime;
        _currentScale = Mathf.Lerp(_peakScale, 0f, t);

        UpdateTransformScale(Mathf.Clamp(_currentScale * spectrum.scaleMultiplier, 1f, float.MaxValue));
    }

    private void UpdateTransformScale(float scale)
    {
        rect.localScale = new Vector3(1f, scale, 1f) * 0.1f;
    }
}
