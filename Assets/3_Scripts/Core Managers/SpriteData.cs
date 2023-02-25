using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteData : MonoBehaviour
{
    public static SpriteData Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType(typeof(SpriteData)) as SpriteData;

            return instance;
        }
        set
        {
            instance = value;
        }
    }
    private static SpriteData instance;

    [Header("Sprite")]
    public Sprite circle;
    public Sprite cross;
    public Sprite square;
    public Sprite triangle;
}
