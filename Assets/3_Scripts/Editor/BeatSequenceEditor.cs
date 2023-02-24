using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BeatSequence))]
public class BeatSequenceEditor : Editor
{
    private const float WindowRatio = 16f / 9f;
    private const float BeatSize = 30f;
    private Texture2D circleTexture, crossTexture, squareTexture, triangleTexture;

    private void OnEnable()
    {
        circleTexture = Resources.Load<Texture2D>("Textures/PS4_Circle");
        crossTexture = Resources.Load<Texture2D>("Textures/PS4_Cross");
        squareTexture = Resources.Load<Texture2D>("Textures/PS4_Square");
        triangleTexture = Resources.Load<Texture2D>("Textures/PS4_Triangle");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // Get the BeatSequence object being inspected
        BeatSequence beatSequence = (BeatSequence)target;

        // Create a rect for the window box
        float windowHeight = EditorGUIUtility.currentViewWidth / WindowRatio;
        Rect windowRect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, windowHeight);
        GUI.Box(windowRect, GUIContent.none);

        // Calculate the size of the simulated quadrant
        float quadrantWidth = windowRect.width / 2f;
        float quadrantHeight = windowRect.height / 2f;

        // Calculate the center of the window box
        Vector2 center = new Vector2(windowRect.xMin + quadrantWidth, windowRect.yMin + quadrantHeight);

        // Draw each BeatSettings object in the window box
        foreach (BeatSettings beat in beatSequence.beatSettings)
        {
            // Calculate the position of the beat in the simulated quadrant
            float x = beat.position.x / 1920f * quadrantWidth;
            float y = beat.position.y / 1080f * quadrantHeight;
            Vector2 beatPos = center + new Vector2(x, -y);

            // Draw a circle at the beat position
            Rect textureRect = new Rect(beatPos.x - BeatSize / 2f, beatPos.y - BeatSize / 2f, BeatSize, BeatSize);
            GUI.DrawTexture(textureRect, GetTextureForInputKey(beat.key));

            // Add a label for the beatSettings index
            Rect labelRect = new Rect(textureRect.x - BeatSize / 2f, textureRect.yMax, 60f, 20f);
            GUI.Label(labelRect, "Beat " + beat.onBeatCount, EditorStyles.centeredGreyMiniLabel);
        }
    }

    private Texture GetTextureForInputKey(KeyInput key)
    {
        switch (key)
        {
            case KeyInput.Circle:
                return circleTexture;
            case KeyInput.Cross:
                return crossTexture;
            case KeyInput.Square:
                return squareTexture;
            case KeyInput.Triangle:
                return triangleTexture;
            default:
                return null;
        }
    }
}
