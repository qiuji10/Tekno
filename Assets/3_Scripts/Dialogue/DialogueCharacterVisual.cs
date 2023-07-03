using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueCharacterVisual : MonoBehaviour
{
    [SerializeField] private Image image;

    public DialogueCharacterSprite spriteData;

    private RectTransform rect;
    private Coroutine animationCoroutine;
    private Color greyColor;

    private void Awake()
    {
        rect = transform as RectTransform;
        ColorUtility.TryParseHtmlString("#646464", out greyColor);
    }

    public void SetImageSprite(string spriteName)
    {
        image.sprite = spriteData.GetSprite(spriteName);
    }

    public void SetImageActive(bool isActive)
    {
        image.color = isActive ? Color.white : greyColor;
    }

    public void Animate(AnimationPattern pattern, float animTime)
    {
        animationCoroutine = StartCoroutine(Animate_Logic(pattern, animTime));
    }

    public void StopAnimate()
    {
        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
    }
    //
    public IEnumerator Animate_Logic(AnimationPattern pattern, float animTime)
    {
        float realTime = 0f;
        float elapsedTime = 0f;
        float curveTime = pattern.GetLongestCurve()[pattern.GetLongestCurve().length - 1].time; // Get the total time of the x position curve

        while (realTime < animTime)
        {
            if (elapsedTime >= curveTime * 0.5f)
            {
                elapsedTime = 0f;
            }

            float t = elapsedTime / curveTime;

            // Evaluate the animation curves for x and y positions
            float xPos = pattern.xPosCurve.Evaluate(t);
            float yPos = pattern.yPosCurve.Evaluate(t);

            // Apply the positions to the rect's anchoredPosition
            rect.anchoredPosition = new Vector2(xPos, yPos);

            elapsedTime += 1f / 60f; // Use a fixed delta time of 1/60th of a second
            realTime += 1f / 60f;

            yield return new WaitForSecondsRealtime(1f / 60f);
        }
    }
}
