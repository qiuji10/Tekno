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
            // Get the curves for the selected animation clip
            AnimationClipCurveData[] curveDatas = AnimationUtility.GetAllCurves(animationClip, true);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            foreach (AnimationClipCurveData curveData in curveDatas)
            {
                AnimationCurve curve = curveData.curve;
                string propertyName = curveData.propertyName;

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
            // Get the curves for the selected animation clip
            AnimationClipCurveData[] curveDatas = AnimationUtility.GetAllCurves(animationClip, true);

            // Iterate through the curves and copy them
            foreach (AnimationClipCurveData curveData in curveDatas)
            {
                AnimationCurve curve = curveData.curve;
                string propertyName = curveData.propertyName;

                // Do whatever you want with the curve data here
                Debug.Log("Property Name: " + propertyName);
                Debug.Log("Curve: " + curve.ToString());

                if (propertyName == curveName)
                    extractedCurve = curve;
            }
        }
    }
}
