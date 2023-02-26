using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BeatSequence))]
public class BeatSequenceEditor : Editor
{
    private const float WindowRatio = 16f / 9f;
    private const float BeatSize = 30f;
    private Texture2D circleTexture, crossTexture, squareTexture, triangleTexture;
    private Rect windowRect;

    private bool isDraggingBeat = false;
    private int beatBeingDragged = -1;

    private SerializedProperty objectProperty;

    private void OnEnable()
    {
        circleTexture = Resources.Load<Texture2D>("Textures/PS4_Circle");
        crossTexture = Resources.Load<Texture2D>("Textures/PS4_Cross");
        squareTexture = Resources.Load<Texture2D>("Textures/PS4_Square");
        triangleTexture = Resources.Load<Texture2D>("Textures/PS4_Triangle");

        objectProperty = serializedObject.FindProperty("beatSettings");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        //base.OnInspectorGUI();
        GUILayout.BeginHorizontal();
        // Get the BeatSequence object being inspected
        BeatSequence beatSequence = (BeatSequence)target;
        // Draw the beatSettings list as usual

        // Create a rect for the window box
        float windowWidth = EditorGUIUtility.currentViewWidth - 5;
        float windowHeight = windowWidth / WindowRatio;
        Rect winRect = new Rect(0, 0, windowWidth, windowHeight);
        Handles.DrawWireCube(winRect.center, new Vector3(winRect.width, winRect.height, 0f));
        //windowRect = GUILayoutUtility.GetRect(winRect);
        GUI.Box(winRect, GUIContent.none);
        
        // Calculate the size of the simulated quadrant
        float quadrantWidth = windowWidth / 2f;
        float quadrantHeight = windowHeight / 2f;

        // Draw each BeatSettings object in the window box
        for (int i = 0; i < beatSequence.beatSettings.Count; i++)
        {
            BeatSettings beat = beatSequence.beatSettings[i];

            // Calculate the position of the beat in the simulated quadrant
            float x = beat.position.x / 1920f * quadrantWidth * 2;
            float y = beat.position.y / 1080f * quadrantHeight * 2;

            Vector2 beatPos = winRect.center + new Vector2(x, -y);

            // Clamp the beat position within the window box
            float minX = winRect.xMin + BeatSize / 2f;
            float maxX = winRect.xMax - BeatSize / 2f;
            float minY = winRect.yMin + BeatSize / 2f;
            float maxY = winRect.yMax - BeatSize / 2f;

            beatPos.x = Mathf.Clamp(beatPos.x, minX - BeatSize, maxX);
            beatPos.y = Mathf.Clamp(beatPos.y, minY - BeatSize, maxY);

            // Draw a circle at the beat position
            Rect textureRect = new Rect(beatPos.x - BeatSize / 2f, beatPos.y - BeatSize / 2f, BeatSize, BeatSize);

            

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

            GUI.DrawTexture(textureRect, GetTextureForInputKey(beat.key));
            
            // Add a label for the beatSettings index
            Rect labelRect = new Rect(textureRect.x - BeatSize / 2f, textureRect.yMax, 60f, 20f);
            GUI.Label(labelRect, "Beat " + beat.onBeatCount, EditorStyles.centeredGreyMiniLabel);
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(windowHeight);

        EditorGUILayout.PropertyField(objectProperty, true);
        serializedObject.ApplyModifiedProperties();
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
