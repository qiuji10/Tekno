using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SpriteInfo
{
    public string spriteName;
    public Sprite sprite;
}

[CreateAssetMenu(menuName = "Dialogue/CharacterSpriteData")]
public class DialogueCharacterSprite : ScriptableObject
{
    public string characterName;
    public List<SpriteInfo> sprites = new List<SpriteInfo>();

    public Sprite GetSprite(string emotionName)
    {
        return sprites.Find(data => data.spriteName == emotionName).sprite;
    }
}