using UnityEngine;

public static class EasingFunctions
{
    public static float EaseOutExpo(float x)
    {
        return x == 1 ? 1 : 1 - Mathf.Pow(2, -10 * x);
    }
}