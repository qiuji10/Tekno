using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatNote : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] CanvasGroup group;
    private Image ring;

    private Coroutine shrinkRoutine;

    private void Awake()
    {
        ring = group.GetComponent<Image>();
    }

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

    public void SetRingColor(Color color)
    {
        ring.color = color;
    }

    public void StopShrink()
    {
        if (shrinkRoutine != null)
            StopCoroutine(shrinkRoutine);
    }

    public void ShrinkRing()
    {
        StartCoroutine(Appear());
        shrinkRoutine = StartCoroutine(Shrink());

        IEnumerator Shrink()
        {
            float timer = 0f;
            float time = 60f / TempoManager.staticBPM;
            Vector3 large = new Vector3(5f, 5f, 5f);

            while (timer < time)
            {
                timer += Time.deltaTime;
                float ratio = timer / time;
                group.transform.localScale = Vector3.Lerp(large, Vector3.one, ratio);
                yield return null;
            }
        }

        IEnumerator Appear()
        {
            float timer = 0f;
            float time = 0.15f;

            while (timer < time)
            {
                timer += Time.deltaTime;
                group.alpha = Mathf.Lerp(0.0f, 1.0f, timer / time);
                yield return null;
            }
        }
    }
}
