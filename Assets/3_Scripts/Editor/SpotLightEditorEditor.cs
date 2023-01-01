using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpotlightEditor))]
public class SpotlightEditorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();

        // Get the target script
        SpotlightEditor targetScript = (SpotlightEditor)target;

        // Check if the script is attached to a GameObject with a Light component
        Light spotlight = targetScript.GetComponent<Light>();
        if (spotlight == null)
        {
            EditorGUILayout.HelpBox("This script must be attached to a GameObject with a Light component.", MessageType.Error);
            return;
        }

        // Check if the GameObject has a child with the name "Cone"
        Transform cone = targetScript.transform.GetChild(0);
        if (cone == null)
        {
            EditorGUILayout.HelpBox("No child 'Cone' was found.", MessageType.Error);
            return;
        }
    }
}
