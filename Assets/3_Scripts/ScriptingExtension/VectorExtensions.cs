using UnityEngine;

public static class VectorExtensions
{
    public static Vector2 Vec2(float value)
    {
        return new Vector2(value, value);
    }

    public static Vector3 Vec3(float value)
    {
        return new Vector3(value, value, value);
    }
}
