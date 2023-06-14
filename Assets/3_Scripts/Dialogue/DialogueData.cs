using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public string characterName;
    [TextArea(10, 20)] public string conversation;
}

[CreateAssetMenu(menuName = "Dialogue/DialogueData")]
public class DialogueData : ScriptableObject
{
    public string dialogueName;

    public List<Dialogue> dialogues = new List<Dialogue>();

    public string[] GetAllCharacter()
    {
        HashSet<string> characterSet = new HashSet<string>();

        for (int i = 0; i < dialogues.Count; i++)
        {
            string characterName = dialogues[i].characterName;
            characterSet.Add(characterName);
        }

        string[] characters = new string[characterSet.Count];
        characterSet.CopyTo(characters);

        return characters;
    }

}
