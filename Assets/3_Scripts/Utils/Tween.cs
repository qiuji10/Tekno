using UnityEngine;

public class Tween : MonoBehaviour
{
    private static Tween instance;

    public static Tween Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("Tween").AddComponent<Tween>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public Coroutine TweenFloat(float from, float to, float duration, System.Action<float> onUpdate, System.Action onComplete, EaseType easeType = EaseType.Linear)
    {
        return StartCoroutine(TweenFloatCoroutine(from, to, duration, onUpdate, onComplete, easeType));
    }

    private System.Collections.IEnumerator TweenFloatCoroutine(float from, float to, float duration, System.Action<float> onUpdate, System.Action onComplete, EaseType easeType)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float easedT = GetEasedValue(t, easeType);
            float currentValue = Mathf.Lerp(from, to, easedT);
            onUpdate.Invoke(currentValue);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        onUpdate.Invoke(to);
        onComplete.Invoke();
    }

    private float GetEasedValue(float t, EaseType easeType)
    {
        switch (easeType)
        {
            case EaseType.Linear:
                return t;
            case EaseType.EaseInQuad:
                return t * t;
            case EaseType.EaseOutQuad:
                return t * (2 - t);
            case EaseType.EaseInOutQuad:
                return t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t;
            // Add more easing functions here
            default:
                return t;
        }
    }
}

public enum EaseType
{
    Linear,
    EaseInQuad,
    EaseOutQuad,
    EaseInOutQuad
    // Add more easing types here
}
