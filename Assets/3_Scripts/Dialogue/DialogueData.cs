using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public string characterName;
    [TextArea(10, 20)] public string conversation;

    public string RemoveRegexFromString()
    {
        Regex waitRegex = new Regex(@"\$wait/([\d.]+)");
        Regex playRegex = new Regex(@"\$play/(\w+)/([\d.]+)");
        Regex emotionRegex = new Regex(@"\$emotion/(\w+)");

        string result = conversation;

        result = waitRegex.Replace(result, string.Empty);
        result = playRegex.Replace(result, string.Empty);
        result = emotionRegex.Replace(result, string.Empty);

        return result;
    }
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

        List<string> characters = new List<string>(characterSet);
        characters.Sort(new CharacterPriorityComparer());

        return characters.ToArray();
    }
}

public class CharacterPriorityComparer : IComparer<string>
{
    private Dictionary<string, int> priorityMap;

    public CharacterPriorityComparer()
    {
        priorityMap = new Dictionary<string, int>
        {
            { "Tekno", 1 },
            { "SP3-KER", 2 }
        };
    }

    public int Compare(string x, string y)
    {
        int xPriority, yPriority;

        if (priorityMap.TryGetValue(x, out xPriority) && priorityMap.TryGetValue(y, out yPriority))
        {
            return xPriority.CompareTo(yPriority);
        }

        return string.Compare(x, y, StringComparison.Ordinal);
    }
}
