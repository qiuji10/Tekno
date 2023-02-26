using Unity.VisualScripting;
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
        //base.OnInspectorGUI();

        serializedObject.Update();

        // Get the BeatSequence object being inspected
        BeatSequence beatSequence = (BeatSequence)target;
        // Draw the beatSettings list as usual
        
        // Create a rect for the window box
        float windowHeight = EditorGUIUtility.currentViewWidth / WindowRatio;
        Rect windowRect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, windowHeight);
        GUI.Box(windowRect, GUIContent.none);

        // Calculate the size of the simulated quadrant
        float quadrantWidth = windowRect.width / 2f;
        float quadrantHeight = windowRect.height / 2f;

        // Calculate the center of the window box
        Vector2 center = new Vector2(windowRect.xMin + quadrantWidth, windowRect.yMin + quadrantHeight);
        SerializedProperty objectProperty = serializedObject.FindProperty("beatSettings");
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
                newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
                newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

                // Draw the texture at the new position
                Rect newRect = new Rect(newPosition.x, newPosition.y, BeatSize, BeatSize);
                textureRect = newRect;

                // Calculate the position of the beat in the simulated quadrant
                float new_x = (newRect.x - center.x) / quadrantWidth * 960f;
                float new_y = (center.y - newRect.y) / quadrantHeight * 540f;

                //beat.position = new Vector2(new_x, new_y);

                //if (EditorGUI.EndChangeCheck())
                positionProperty.vector2Value = new Vector2(new_x, new_y);
                Repaint();
                Debug.Log($"2 {serializedObject.ApplyModifiedProperties()}");
                //EditorUtility.SetDirty(target);
                //Debug.Log(beat.position);
                
            }

            GUI.DrawTexture(textureRect, GetTextureForInputKey(beat.key));
            
            // Add a label for the beatSettings index
            Rect labelRect = new Rect(textureRect.x - BeatSize / 2f, textureRect.yMax, 60f, 20f);
            GUI.Label(labelRect, "Beat " + beat.onBeatCount, EditorStyles.centeredGreyMiniLabel);
        }

        
        //EditorGUI.BeginChangeCheck();

        //SerializedProperty beatSettingsProp = serializedObject.FindProperty("beatSettings");
        EditorGUILayout.PropertyField(objectProperty, true);


        serializedObject.ApplyModifiedProperties();
        //if (EditorGUI.EndChangeCheck())
        //{
        //    EditorUtility.SetDirty(target);
        //}
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
