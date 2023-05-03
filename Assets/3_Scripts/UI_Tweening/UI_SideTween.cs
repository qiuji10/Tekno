using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UI_SideTween : MonoBehaviour
{
    [SerializeField] Ease ease;
    [SerializeField] float duration;

    [SerializeField] AnimationCurve curve;

    RectTransform element;

    void Awake()
    {
        element = GetComponent<RectTransform>();
    }

    public void MoveX(float position)
    {
        StartCoroutine(MovingLeftRight(position));
    }

    IEnumerator MovingLeftRight(float position)
    {
        element.DOMoveX(position, duration).SetEase(ease);
        yield return null;
    }

    public void MoveX_InElastic(float position)
    {
        StartCoroutine(MenuButtonClicked(position));
    }

    IEnumerator MenuButtonClicked(float position)
    {
        element.DOMoveX(position, duration).SetEase(curve);
        yield return null;
    }

    public void Enlarge(float scale)
    {
        StartCoroutine(EnlargeElement(scale));
    }

    IEnumerator EnlargeElement(float scale)
    {
        yield return new WaitForSeconds(duration / 2);
        transform.LeanScale(new Vector3(scale, scale, 1), duration);
        yield return null;
    }
}
