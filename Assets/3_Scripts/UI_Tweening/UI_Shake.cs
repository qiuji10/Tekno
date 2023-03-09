using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using NaughtyAttributes;

public class UI_Shake : MonoBehaviour
{
    public float duration = 0.5f;
    public float shakeAmount = 10f;
    public float shakeFrequency = 25f;

    private RectTransform uiElement;

    private void Awake()
    {
        uiElement = GetComponent<RectTransform>();
    }

    private IEnumerator ShakeCoroutine()
    {
        Vector3 originalPos = uiElement.localPosition;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float x = originalPos.x + Random.Range(-1f, 1f) * shakeAmount;
            float y = originalPos.y + Random.Range(-1f, 1f) * shakeAmount;
            uiElement.localPosition = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        uiElement.localPosition = originalPos;
    }

    [Button]
    public void Shake()
    {
        StartCoroutine(ShakeCoroutine());
    }
}
