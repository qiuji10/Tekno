using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BeatSequence))]
public class BeatSequenceEditor : Editor
{
    private const float WindowRatio = 16f / 9f;
    private const float BeatSize = 30f;
    private Texture2D circleTexture, crossTexture, squareTexture, triangleTexture;

    private bool isDraggingBeat = false;
    private int beatBeingDragged = -1;

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
        for (int i = 0; i < beatSequence.beatSettings.Count; i++)
        {
            BeatSettings beat = beatSequence.beatSettings[i];

            // Calculate the position of the beat in the simulated quadrant
            float x = beat.position.x / 1920f * quadrantWidth * 2;
            float y = beat.position.y / 1080f * quadrantHeight * 2;
            Vector2 beatPos = center + new Vector2(x, -y);

            // Clamp the beat position within the window box
            float minX = windowRect.xMin + BeatSize / 2f;
            float maxX = windowRect.xMax - BeatSize / 2f;
            float minY = windowRect.yMin + BeatSize / 2f;
            float maxY = windowRect.yMax - BeatSize / 2f;

            beatPos.x = Mathf.Clamp(beatPos.x, minX, maxX);
            beatPos.y = Mathf.Clamp(beatPos.y, minY, maxY);

            // Draw a circle at the beat position
            Rect textureRect = new Rect(beatPos.x - BeatSize / 2f, beatPos.y - BeatSize / 2f, BeatSize, BeatSize);

            if (Event.current.type == EventType.MouseDown && textureRect.Contains(Event.current.mousePosition))
            {
                isDraggingBeat = true;
                beatBeingDragged = i;
                Event.current.Use();
            }
            else if (Event.current.type == EventType.MouseUp && isDraggingBeat && beatBeingDragged == i)
            {
                isDraggingBeat = false;
                beatBeingDragged = -1;
                Event.current.Use();
            }
            else if (isDraggingBeat && beatBeingDragged == i)
            {
                // Calculate the new position of the beat texture based on the mouse position
                Vector2 mousePos = Event.current.mousePosition;

                float xRatio = (mousePos.x - windowRect.xMin) / windowRect.width;
                float yRatio = (mousePos.y - windowRect.yMin) / windowRect.height;
                //Debug.Log(quadrantWidth);
                float xPos = Mathf.Lerp(minX, maxX, xRatio);
                float yPos = Mathf.Lerp(minY, maxY, yRatio); // Y-axis is inverted in Unity Editor window

                Vector2 mousePosInBox = new Vector2(xPos, yPos);
                Debug.Log(mousePosInBox);
                beat.position = new Vector2(xPos, yPos);
            }


            GUI.DrawTexture(textureRect, GetTextureForInputKey(beat.key));

            // Add a label for the beatSettings index
            Rect labelRect = new Rect(textureRect.x - BeatSize / 2f, textureRect.yMax, 60f, 20f);
            GUI.Label(labelRect, "Beat " + beat.onBeatCount, EditorStyles.centeredGreyMiniLabel);
        }

        //Debug.Log(Event.current.mousePosition);
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
