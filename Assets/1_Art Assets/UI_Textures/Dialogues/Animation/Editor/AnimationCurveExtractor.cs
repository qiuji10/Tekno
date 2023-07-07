using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Newtonsoft.Json;

public class AnimationCurveExtractor : EditorWindow
{
    private string curveName;
    private AnimationCurve extractedCurve;
    private Vector2 scrollPosition;

    [MenuItem("Window/Animation Curve Extractor")]
    static void Init()
    {
        AnimationCurveExtractor window = (AnimationCurveExtractor)EditorWindow.GetWindow(typeof(AnimationCurveExtractor));
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Extracted Curve:", EditorStyles.boldLabel);
        curveName = EditorGUILayout.TextField("Curve Name", curveName);
        extractedCurve = EditorGUILayout.CurveField("Curve", extractedCurve);

        if (GUILayout.Button("Find Animation Curve"))
        {
            ExtractCurve();
        }

        GUILayout.Space(20);
        GUILayout.Label("Available Animation Curves:", EditorStyles.boldLabel);

        // Get the selected animation clip
        AnimationClip animationClip = Selection.activeObject as AnimationClip;

        if (animationClip != null)
        {
            // Get the curve bindings for the selected animation clip
            EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings(animationClip);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            foreach (EditorCurveBinding curveBinding in curveBindings)
            {
                AnimationCurve curve = AnimationUtility.GetEditorCurve(animationClip, curveBinding);
                string propertyName = curveBinding.propertyName;

                if (GUILayout.Button(propertyName))
                {
                    curveName = propertyName;
                    extractedCurve = curve;
                }
            }

            EditorGUILayout.EndScrollView();
        }

    }

    private void ExtractCurve()
    {
        // Get the selected animation clip
        AnimationClip animationClip = Selection.activeObject as AnimationClip;

        if (animationClip != null)
        {
            // Get the curve bindings for the selected animation clip
            EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings(animationClip);

            // Iterate through the curve bindings and copy the curves
            foreach (EditorCurveBinding curveBinding in curveBindings)
            {
                AnimationCurve curve = AnimationUtility.GetEditorCurve(animationClip, curveBinding);
                string propertyName = curveBinding.propertyName;

                // Do whatever you want with the curve data here
                Debug.Log("Property Name: " + propertyName);
                Debug.Log("Curve: " + curve.ToString());

                if (propertyName == curveName)
                    extractedCurve = curve;
            }
        }

    }
}
