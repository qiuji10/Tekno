using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class SDSToJSONTranslator
{
    public static string TranslateSDSToJSON(TextAsset sdsTextAsset)
    {
        string sdsText = sdsTextAsset.text;
        DialogueData dialogueData = ParseSDSText(sdsText);
        string jsonText = JsonUtility.ToJson(dialogueData, true);
        return jsonText;
    }

    public static void TranslateSDSToJSON(string sdsFilePath, string jsonFilePath)
    {
        string sdsText = File.ReadAllText(sdsFilePath);
        DialogueData dialogueData = ParseSDSText(sdsText);
        string jsonText = JsonUtility.ToJson(dialogueData, true);
        File.WriteAllText(jsonFilePath, jsonText);
    }

    private static DialogueData ParseSDSText(string sdsText)
    {
        DialogueData dialogueData = new DialogueData();

        string[] lines = sdsText.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

        string dialogueName = ParseDialogueName(lines[0]);
        dialogueData.dialogueName = dialogueName;

        List<Dialogue> dialogues = new List<Dialogue>();

        for (int i = 2; i < lines.Length; i++)
        {
            if (lines[i] == "--")
                continue;

            string[] parts = lines[i].Split(new[] { ':' }, 2);
            if (parts.Length == 2)
            {
                string characterName = parts[0].Trim();
                string conversation = parts[1].Trim();

                Dialogue dialogue = new Dialogue
                {
                    characterName = characterName,
                    conversation = conversation
                };
                dialogues.Add(dialogue);
            }
            else
            {
                throw new FormatException("Invalid SDS format: missing character name or conversation.");
            }
        }

        dialogueData.dialogues = dialogues;
        return dialogueData;
    }

    private static string ParseDialogueName(string line)
    {
        string pattern = @"Title:\s*(.+)";
        Match match = Regex.Match(line, pattern);
        if (match.Success)
        {
            return match.Groups[1].Value.Trim();
        }
        else
        {
            throw new FormatException("Invalid SDS format: missing dialogue name.");
        }
    }
}
