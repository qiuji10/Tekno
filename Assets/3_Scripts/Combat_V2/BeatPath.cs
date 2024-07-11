using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class BeatPath : MonoBehaviour
{
    [Header("[Sliders]")]
    [SerializeField] private CustomSlider previewSlider;
    [SerializeField] private CustomSlider pathSlider;

    [Header("[Rect Transforms]")]
    [SerializeField] private RectTransform rect;
    [SerializeField] private RectTransform start;
    [SerializeField] private RectTransform end;

#if UNITY_EDITOR
    private void Update()
    {
        UpdatePath();
    }
#endif

    public void SetPosition(Vector2 startPos, Vector2 endPos)
    {
        start.anchoredPosition = startPos;
        end.anchoredPosition = endPos;

        UpdatePath();
    }

    public void SetPreviewValue(float from, float to, float time)
    {
        previewSlider.Lerp(from, to, time);
    }

    public void SetPathValue(float from, float to, float time)
    {
        pathSlider.Lerp(from, to, time);
    }

    public void Cancel()
    {
        pathSlider.StopLerp();
    }

    private void UpdatePath()
    {
        if (rect != null && start != null && end != null)
        {
            Vector3 centerPos = (start.anchoredPosition + end.anchoredPosition) / 2;
            rect.anchoredPosition = centerPos;

            Vector2 direction = (start.anchoredPosition - end.anchoredPosition);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            rect.eulerAngles = new Vector3(0, 0, angle);

            rect.sizeDelta = new Vector2(direction.magnitude, rect.sizeDelta.y);
        }
    }
}
