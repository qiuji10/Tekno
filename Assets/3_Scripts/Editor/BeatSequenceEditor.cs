using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BeatSequence))]
public class BeatSequenceEditor : Editor
{
    private const float WindowRatio = 16f / 9f;
    private const float BeatSize = 30f;
    private Texture2D circleTexture, crossTexture, squareTexture, triangleTexture, skipTexture, speakerTex;

    private bool isDraggingBeat = false;
    private int beatBeingDragged = -1;

    private SerializedProperty objectProperty;
    private Vector2 scrollPosition;
    private Vector2 speakerPos = new Vector2(-768.55f, 0f);

    private void OnEnable()
    {
        circleTexture = Resources.Load<Texture2D>("Textures/PS4_Circle");
        crossTexture = Resources.Load<Texture2D>("Textures/PS4_Cross");
        squareTexture = Resources.Load<Texture2D>("Textures/PS4_Square");
        triangleTexture = Resources.Load<Texture2D>("Textures/PS4_Triangle");
        skipTexture = Resources.Load<Texture2D>("Textures/skip");
        speakerTex = Resources.Load<Texture2D>("Textures/Speaker");

        objectProperty = serializedObject.FindProperty("beatSettings");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        
        // Get the BeatSequence object being inspected
        BeatSequence beatSequence = (BeatSequence)target;

        // Create a rect for the window box
        float windowWidth = EditorGUIUtility.currentViewWidth - 10;
        float windowHeight = windowWidth / WindowRatio;
        Rect winRect = new Rect(5, 0, windowWidth, windowHeight);
        Handles.DrawWireCube(winRect.center, new Vector3(winRect.width, winRect.height, 0f));

        GUI.Box(winRect, GUIContent.none);

        // Calculate the size of the simulated quadrant
        float quadrantWidth = windowWidth / 2f;
        float quadrantHeight = windowHeight / 2f;

        DrawSpeaker(winRect, quadrantWidth, quadrantHeight);

        // Draw each BeatSettings object in the window box
        for (int i = 0; i < beatSequence.beatSettings.Count; i++)
        {
            BeatData beat = beatSequence.beatSettings[i];

            // Clamp the beat position within the window box
            float minX = winRect.xMin + BeatSize / 2f;
            float maxX = winRect.xMax - BeatSize / 2f;
            float minY = winRect.yMin + BeatSize / 2f;
            float maxY = winRect.yMax - BeatSize / 2f;

            // Draw a circle at the beat position
            Rect textureRect = DrawRect(winRect, beat.position, quadrantWidth, quadrantHeight);

            SerializedProperty elementProperty = objectProperty.GetArrayElementAtIndex(i);
            SerializedProperty positionProperty = elementProperty.FindPropertyRelative("position");

            if (Event.current.type == EventType.MouseDown && textureRect.Contains(Event.current.mousePosition))
            {
                isDraggingBeat = true;
                beatBeingDragged = i;
            }
            else if (Event.current.type == EventType.MouseUp && isDraggingBeat && beatBeingDragged == i)
            {
                isDraggingBeat = false;
                beatBeingDragged = -1;
            }
            else if (isDraggingBeat && beatBeingDragged == i)
            {
                // Calculate the new position of the dragged beat based on mouse position
                Vector2 newPosition = Event.current.mousePosition - new Vector2(BeatSize / 2, BeatSize / 2);
                newPosition.x = Mathf.Clamp(newPosition.x, minX - BeatSize, maxX);
                newPosition.y = Mathf.Clamp(newPosition.y, minY - BeatSize, maxY);

                Vector2Int intNewPos = Vector2Int.RoundToInt(newPosition);

                // Draw the texture at the new position
                Rect newRect = new Rect(intNewPos.x, intNewPos.y, BeatSize, BeatSize);
                textureRect = newRect;

                // Calculate the position of the beat in the simulated quadrant
                float new_x = (newRect.position.x - winRect.center.x) / quadrantWidth * 960f;
                float new_y = (winRect.center.y - newRect.position.y) / quadrantHeight * 540f;

                Vector2 updatedPos = new Vector2(new_x + BeatSize * 1.6f, new_y - BeatSize * 1.6f);
                Vector2Int intUpdatePos = Vector2Int.RoundToInt(updatedPos);
                positionProperty.vector2Value = intUpdatePos;

                serializedObject.ApplyModifiedProperties();

                Repaint();
            }

            
            if (i < beatSequence.beatSettings.Count - 1)
            {
                BeatData currentBeat = beatSequence.beatSettings[i];
                BeatData nextBeat = beatSequence.beatSettings[i + 1];

                // Calculate the positions of the current and next beats
                float x1 = currentBeat.position.x / 1920f * quadrantWidth * 2;
                float y1 = currentBeat.position.y / 1080f * quadrantHeight * 2;
                Vector2 currentPos = winRect.center + new Vector2(x1, -y1);

                float x2 = nextBeat.position.x / 1920f * quadrantWidth * 2;
                float y2 = nextBeat.position.y / 1080f * quadrantHeight * 2;
                Vector2 nextPos = winRect.center + new Vector2(x2, -y2);

                // Draw a line between the two beats
                Handles.color = Color.red;
                Handles.DrawLine(currentPos, nextPos);
            }

            GUI.DrawTexture(textureRect, GetTextureForInputKey(beat.key));
            
            // Add a label for the beatSettings index
            Rect labelRect = new Rect(textureRect.x - BeatSize / 2f, textureRect.yMax, 60f, 20f);
            GUI.Label(labelRect, "Beat " + i, EditorStyles.centeredGreyMiniLabel);
        }
        

        GUILayout.Space(windowHeight + 10);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(450));
        EditorGUILayout.BeginVertical();

        //GUI.backgroundColor = Color.cyan;
        EditorGUILayout.PropertyField(objectProperty);

        serializedObject.ApplyModifiedProperties();


        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();

    }

    private void DrawSpeaker(Rect winRect, float quadrantWidth, float quadrantHeight)
    {
        Rect speakerRect = DrawRect(winRect, speakerPos, quadrantWidth, quadrantHeight);
        GUI.DrawTexture(speakerRect, speakerTex);
    }

    private Rect DrawRect(Rect winRect, Vector2 pos, float quadrantWidth, float quadrantHeight)
    {
        float x = pos.x / 1920f * quadrantWidth * 2;
        float y = pos.y / 1080f * quadrantHeight * 2;

        Vector2 beatPos = winRect.center + new Vector2(x, -y);

        // Clamp the beat position within the window box
        float minX = winRect.xMin + BeatSize / 2f;
        float maxX = winRect.xMax - BeatSize / 2f;
        float minY = winRect.yMin + BeatSize / 2f;
        float maxY = winRect.yMax - BeatSize / 2f;

        beatPos.x = Mathf.Clamp(beatPos.x, minX - BeatSize, maxX);
        beatPos.y = Mathf.Clamp(beatPos.y, minY - BeatSize, maxY);

        return new Rect(beatPos.x - BeatSize / 2f, beatPos.y - BeatSize / 2f, BeatSize, BeatSize);
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
            case KeyInput.None:
                return skipTexture;
            default:
                return null;
        }
    }
}
