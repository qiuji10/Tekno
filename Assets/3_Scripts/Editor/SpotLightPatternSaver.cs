using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Splines;

public class SpotLightPatternSaver : EditorWindow
{
    private string savePath = "Assets/3_Scripts/Stage/Light/SpotLightPattern";

    // List of splines to display in the window
    private List<Spline> splines = new List<Spline>();
    private List<GameObject> selectedGO = new List<GameObject>();
    // ScriptableObject to save the data to
    private SpotLightPattern pattern;

    [MenuItem("Tools/SpotLightPatternSaver")]
    public static void ShowWindow()
    {
        GetWindow<SpotLightPatternSaver>("SpotLightPatternSaver");
    }

    private void OnGUI()
    {
        if (splines.Count > 0)
            EditorGUILayout.HelpBox($"Currently have {splines.Count} of Spline gameObject selected", MessageType.Info);
        else
            EditorGUILayout.HelpBox($"No gameObject with Spline selected", MessageType.Warning);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Save Path");
        EditorGUILayout.TextField(savePath);

        EditorGUILayout.Space();

        {
            int i = 0;
            // Display each spline in the window
            foreach (Spline spline in splines)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField(selectedGO[i], typeof(GameObject), true);
                i++;
                EditorGUI.EndDisabledGroup();
            }
        }

        GUIContent buttonContent = new GUIContent("Save Spline Data", "Save DATA to SpotLight Pattern Scriptable Object");
        if (GUILayout.Button(buttonContent))
        {
            // Create a new SpotLightPattern ScriptableObject
            pattern = ScriptableObject.CreateInstance<SpotLightPattern>();
            pattern.splines = new List<Spline>();
            foreach (Spline spline in splines)
            {
                pattern.splines.Add(spline);
            }

            // Save the SpotLightPattern ScriptableObject to the project
            AssetDatabase.CreateAsset(pattern, $"{savePath}/SpotLightPattern.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = pattern;
        }

    }

    private void OnSelectionChange()
    {
        // Clear the splines list
        splines.Clear();
        selectedGO.Clear();
        // Get the selected GameObjects
        GameObject[] selection = Selection.gameObjects;

        // Add any GameObjects with a Spline component to the splines list
        foreach (GameObject go in selection)
        {
            SplineContainer splineContainer = go.GetComponent<SplineContainer>();
            if (splineContainer != null)
            {
                selectedGO.Add(go);
                splines.Add(splineContainer.Spline);
                Repaint();
            }
        }
    }
}