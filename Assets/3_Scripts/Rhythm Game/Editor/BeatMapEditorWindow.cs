using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public enum EditorSelectionType
{
    None,
    Selector,
    AddTapNote
}

public class BeatMapEditorWindow : EditorWindow
{
    public BeatMap beatmap;
    private Vector2Int canvasSize = new Vector2Int(6, 10);
    private Vector2Int noteSize = new Vector2Int(50, 10);

    private Vector2 scrollPos;
    private Rect leftPanelRect;
    private Rect rightPanelRect;
    private Rect beatMapCanvasRect;

    private EditorSelectionType selectionType;
    private Vector2 selectorPosition;
    private Vector2 ghostNotePosition;
    private bool isHoveringCanvas;

    private bool isSelectingNote;
    private NoteData selectedNote;

    [SerializeField] private int positionSlots = 100;

    [SerializeField] private Color laneColor = Color.grey;
    [SerializeField] private Color laneOutlineColor = Color.black;
    [SerializeField] private Color beatsPerBarColor = Color.red;
    [SerializeField] private Color divisionColor = Color.black;
    [SerializeField] private Color ghostBlockColor = Color.cyan;

    [MenuItem("Window/Beat Map Editor")]
    public static void OpenWindow()
    {
        var window = GetWindow<BeatMapEditorWindow>();
        window.titleContent = new GUIContent("Beat Map Editor");
        window.Show();
    }

    private void OnGUI()
    {
        DrawLeftPanel();
        DrawRightPanel();

        if (GUI.changed && beatmap != null)
        {
            EditorUtility.SetDirty(beatmap);
        }
    }

    [OnOpenAsset(1)]
    public static bool OpenGameStateWindow(int instanceID, int line)
    {
        // Check if the asset being opened is of type BeatMap
        BeatMap beatmap = EditorUtility.InstanceIDToObject(instanceID) as BeatMap;
        if (beatmap == null)
            return false;

        bool windowIsOpen = EditorWindow.HasOpenInstances<BeatMapEditorWindow>();

        if (!windowIsOpen)
        {
            BeatMapEditorWindow window = EditorWindow.CreateWindow<BeatMapEditorWindow>();
            window.beatmap = beatmap;
        }
        else
        {
            EditorWindow.FocusWindowIfItsOpen<BeatMapEditorWindow>();
        }

        // Window should now be open, proceed to next step to open file
        return false;
    }

    private void DrawLeftPanel()
    {
        leftPanelRect = new Rect(0, 0, Mathf.Max(position.width * 0.3f, 300f), position.height);

        GUILayout.BeginArea(leftPanelRect, EditorStyles.helpBox);
        GUILayout.Space(10);

        EditorGUILayout.LabelField("Beat Map Editor", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        beatmap = EditorGUILayout.ObjectField("Beat Map", beatmap, typeof(BeatMap), false) as BeatMap;

        if (beatmap != null)
        {
            EditorGUILayout.LabelField("BPM: " + beatmap.bpm);
            EditorGUILayout.LabelField("Time Signature: " + beatmap.timeSignature.x + "/" + beatmap.timeSignature.y);
            EditorGUILayout.LabelField("Division: " + beatmap.division);
            EditorGUILayout.LabelField("Notes Count: " + beatmap.notes.Count);
        }

        DrawHorizontalLine();

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Slots Settings", EditorStyles.boldLabel);
        positionSlots = EditorGUILayout.IntField("Position Slots", positionSlots);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Selection Type", EditorStyles.boldLabel);
        selectionType = (EditorSelectionType)GUILayout.SelectionGrid((int)selectionType, Enum.GetNames(typeof(EditorSelectionType)), 1, EditorStyles.radioButton);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Color Settings", EditorStyles.boldLabel);
        laneColor = EditorGUILayout.ColorField("Lane", laneColor);
        laneOutlineColor = EditorGUILayout.ColorField("Lane Outline", laneOutlineColor);
        beatsPerBarColor = EditorGUILayout.ColorField("Beats Per Bar Line", beatsPerBarColor);
        divisionColor = EditorGUILayout.ColorField("Division Line", divisionColor);
        ghostBlockColor = EditorGUILayout.ColorField("Ghost Block", ghostBlockColor);

        GUILayout.EndArea();

        
    }

    private void DrawRightPanel()
    {
        rightPanelRect = new Rect(leftPanelRect.width, 0, position.width - leftPanelRect.width, position.height);

        GUILayout.BeginArea(rightPanelRect, EditorStyles.helpBox);
        GUILayout.Space(10);

        if (beatmap != null)
        {
            EditorGUILayout.LabelField("Beat Map Canvas", EditorStyles.boldLabel);
            EditorGUILayout.Space();


            Rect scrollViewRect = GUILayoutUtility.GetRect(rightPanelRect.width, rightPanelRect.height);

            float topOffset = -20f; // Adjust this value as needed
            beatMapCanvasRect = new Rect(-30, -topOffset, canvasSize.x * 60, positionSlots * 20 + topOffset);

            scrollPos = GUI.BeginScrollView(scrollViewRect, scrollPos, beatMapCanvasRect);
            {
                GUI.Box(beatMapCanvasRect, GUIContent.none);

                Event e = Event.current;

                HandleMouseMove(e);

                switch (e.type)
                {
                    case EventType.MouseDown:
                        HandleMouseDown(e);
                        break;
                    case EventType.MouseUp:
                        HandleMouseUp(e);
                        break;
                    case EventType.MouseDrag:
                        HandleMouseDrag(e);
                        break;
                    
                }

                DrawLanes();
                DrawLineIndicators();
                DrawBeatMapNotes();
                //DrawPositionNumbers();
                DrawBeatsNumbers();
                DrawGhostNote();
                Repaint();
            }
            GUI.EndScrollView();
        }

        GUILayout.EndArea();
    }

    private void DrawHorizontalLine()
    {
        GUILayout.Box("", GUILayout.Height(2f), GUILayout.ExpandWidth(true));
    }

    private void HandleMouseDown(Event e)
    {
        if (e.button == 1) // Right-click
        {
            if (beatmap != null && selectionType != EditorSelectionType.None)
            {
                Vector2 clickPosition = e.mousePosition;
                NoteData noteToDelete = GetNoteAtPosition(clickPosition);

                if (noteToDelete != null)
                {
                    beatmap.notes.Remove(noteToDelete);
                    Repaint();
                }
            }
        }
        else // Other mouse buttons
        {
            if (selectionType == EditorSelectionType.Selector)
            {
                Vector2 clickPosition = e.mousePosition;
                selectedNote = GetNoteAtPosition(clickPosition);
                isSelectingNote = (selectedNote != null);
            }
            else if (selectionType == EditorSelectionType.AddTapNote && isHoveringCanvas)
            {
                Vector2 clickPosition = e.mousePosition;
                int tapPosition = GetTapPosition(clickPosition);
                int lane = GetNearestLane(clickPosition);
                AddTapNoteToBeatMap(tapPosition, (Lane)lane);
            }
        }
    }


    private void HandleMouseUp(Event e)
    {
        if (selectionType == EditorSelectionType.Selector && isSelectingNote)
        {
            if (selectedNote != null)
            {
                Vector2 releasePosition = e.mousePosition;
                int tapPosition = GetTapPosition(releasePosition);
                int lane = GetNearestLane(releasePosition);
                UpdateNotePosition(selectedNote, tapPosition, (Lane)lane);
            }
        }

        isSelectingNote = false;
        selectedNote = null;
    }

    private void HandleMouseDrag(Event e)
    {
        if (selectionType == EditorSelectionType.Selector && isSelectingNote)
        {
            if (selectedNote != null)
            {
                Vector2 dragPosition = e.mousePosition;
                int tapPosition = GetTapPosition(dragPosition);
                int lane = GetNearestLane(dragPosition);
                UpdateNotePosition(selectedNote, tapPosition, (Lane)lane);
            }
        }
    }

    private void HandleMouseMove(Event e)
    {
        isHoveringCanvas = beatMapCanvasRect.Contains(e.mousePosition);

        if (selectionType == EditorSelectionType.AddTapNote)
        {
            Vector2 dragPosition = e.mousePosition;
            int tapPosition = GetTapPosition(dragPosition);
            int lane = GetNearestLane(dragPosition);
            ghostNotePosition = GetNotePosition(lane, tapPosition);
        }
    }

    private void DrawLanes()
    {
        float laneWidth = beatMapCanvasRect.width / 4f;

        for (int i = 3; i >= 0; i--)
        {
            Rect laneRect = new Rect(i * laneWidth, 0, laneWidth, beatMapCanvasRect.height);
            Handles.DrawSolidRectangleWithOutline(laneRect, laneColor, laneOutlineColor);

            Handles.Label(new Vector3(i * laneWidth, beatMapCanvasRect.height + 10), i.ToString());
        }
    }

    private void DrawPositionNumbers()
    {
        float noteHeight = beatMapCanvasRect.height / positionSlots;

        int divLineCount = positionSlots;

        for (int i = 0; i <= divLineCount; i++)
        {
            float y = beatMapCanvasRect.height - (i * noteHeight) - noteHeight / 2f;
            EditorGUI.LabelField(new Rect(-25, y - 15, 40, 20), i.ToString());
        }
    }

    private void DrawBeatsNumbers()
    {
        float noteHeight = beatMapCanvasRect.height / positionSlots;
        int divLineCount = positionSlots;
        int previousBeatNum = -1;

        for (int i = 0; i <= divLineCount; i++)
        {
            int beatNum = (i / (int)beatmap.division) + 1;

            if (beatNum != previousBeatNum)
            {
                float y = beatMapCanvasRect.height - ((i + 2) * noteHeight) + noteHeight / 2f;
                EditorGUI.LabelField(new Rect(-25, y - 15, 40, 20), beatNum.ToString());
                previousBeatNum = beatNum;
            }
        }
    }


    private void DrawLineIndicators()
    {
        float noteHeight = beatMapCanvasRect.height / positionSlots;

        int divLineCount = positionSlots;

        int previousBeatNum = -1;

        for (int i = 0; i <= divLineCount; i++)
        {
            int beatNum = ((i - 1) / ((int)beatmap.division)) + 1;

            float y = beatMapCanvasRect.height - (i * noteHeight) - noteHeight / 4f;

            Handles.color = beatNum != previousBeatNum + 1? divisionColor : beatsPerBarColor;
            Handles.DrawLine(new Vector3(0, y), new Vector3(beatMapCanvasRect.width, y));

            previousBeatNum = beatNum;
        }
    }

    private void DrawBeatMapNotes()
    {
        foreach (NoteData note in beatmap.notes)
        {
            Vector2 position = GetNotePosition((int)note.lane, note.tapPosition);
            GUI.Box(new Rect(position.x, position.y, noteSize.x, noteSize.y), GUIContent.none, EditorStyles.textField);
        }
    }

    private void DrawGhostNote()
    {
        if (selectionType == EditorSelectionType.AddTapNote && isHoveringCanvas)
        {
            Color ghostColor = ghostBlockColor;

            GUIStyle ghostStyle = new GUIStyle(EditorStyles.textArea);
            ghostStyle.normal.background = MakeTexture(ghostColor);

            GUI.Box(new Rect(ghostNotePosition.x, ghostNotePosition.y, noteSize.x, noteSize.y), GUIContent.none, ghostStyle);
        }
    }

    private void UpdateNotePosition(NoteData note, int tapPosition, Lane lane)
    {
        note.tapPosition = tapPosition;
        note.lane = lane;
    }

    private void AddTapNoteToBeatMap(int tapPosition, Lane lane)
    {
        NoteData note = new NoteData();
        note.type = NoteType.Tap;
        note.tapPosition = tapPosition;
        note.lane = lane;

        beatmap.notes.Add(note);
    }

    private NoteData GetNoteAtPosition(Vector2 position)
    {
        foreach (NoteData note in beatmap.notes)
        {
            Vector2 notePosition = GetNotePosition((int)note.lane, note.tapPosition);
            Rect noteRect = new Rect(notePosition.x, notePosition.y, noteSize.x, noteSize.y);

            if (noteRect.Contains(position))
            {
                return note;
            }
        }

        return null;
    }

    private Vector2 GetNotePosition(int lane, int tapPosition)
    {
        float laneWidth = beatMapCanvasRect.width / 4f;
        float noteHeight = beatMapCanvasRect.height / positionSlots;

        float x = lane * laneWidth + (laneWidth - noteSize.x) / 2;
        float y = beatMapCanvasRect.height - ((tapPosition + 1) * noteHeight);

        return new Vector2(x, y);
    }

    private int GetTapPosition(Vector2 position)
    {
        float noteHeight = beatMapCanvasRect.height / positionSlots;
        int tapPosition = Mathf.FloorToInt((beatMapCanvasRect.height - position.y) / noteHeight);
        return tapPosition;
    }

    private int GetNearestLane(Vector2 position)
    {
        float laneWidth = beatMapCanvasRect.width / 4f;
        int lane = Mathf.FloorToInt(position.x / laneWidth);
        return Mathf.Clamp(lane, 0, 3);
    }

    //private int GetTotalCanvasHeight()
    //{
    //    int highestPosition = 0;

    //    foreach (NoteData note in beatmap.notes)
    //    {
    //        if (note.tapPosition > highestPosition)
    //        {
    //            highestPosition = note.tapPosition;
    //        }
    //    }

    //    return highestPosition;
    //}

    private Texture2D MakeTexture(Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        return texture;
    }
}