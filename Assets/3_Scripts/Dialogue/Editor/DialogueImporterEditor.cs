using UnityEditor;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class DialogueImporterEditor : EditorWindow
{
    private TextAsset dialogueTextAsset;

    [MenuItem("Window/Dialogue Importer")]
    public static void ShowWindow()
    {
        GetWindow<DialogueImporterEditor>("Dialogue Importer");
    }

    private void OnGUI()
    {
        EditorGUILayout.HelpBox("Select a dialogue text file (.txt) to import as a ScriptableObject.", MessageType.Info);
        dialogueTextAsset = EditorGUILayout.ObjectField("Dialogue Text Asset", dialogueTextAsset, typeof(TextAsset), false) as TextAsset;

        EditorGUI.BeginDisabledGroup(dialogueTextAsset == null);
        if (GUILayout.Button("Import Dialogue"))
        {
            ImportDialogue();
        }
        EditorGUI.EndDisabledGroup();
    }

    private void ImportDialogue()
    {
        if (dialogueTextAsset == null)
        {
            Debug.LogError("No dialogue file selected.");
            return;
        }

        string dialogueFilePath = AssetDatabase.GetAssetPath(dialogueTextAsset);
        string dialogueFileName = Path.GetFileNameWithoutExtension(dialogueFilePath);

        string json = SDSToJSONTranslator.TranslateSDSToJSON(dialogueTextAsset);

        DialogueData dialogueData = ScriptableObject.CreateInstance<DialogueData>();
        dialogueData = JsonConvert.DeserializeObject<DialogueData>(json);

        if (dialogueData != null)
        {
            string scriptableObjectPath = Path.Combine("Assets/3_Scripts/Dialogue/DialogueSO", dialogueFileName + ".asset");
            AssetDatabase.CreateAsset(dialogueData, scriptableObjectPath);
            AssetDatabase.SaveAssets();

            Debug.Log("Dialogue imported successfully as a ScriptableObject.");
        }
        else
        {
            Debug.LogError("Failed to create ScriptableObject for the dialogue.");
        }
    }
}
