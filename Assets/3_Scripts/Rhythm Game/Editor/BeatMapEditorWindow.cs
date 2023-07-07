using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public enum EditorSelectionType
{
    None,
    Selector,
    Add_Tap_Note,
    Add_Hold_Note,
}

public enum EditorSelectionNotePart
{
    None,
    TapPosition,
    HoldToPosition
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
    private EditorSelectionNotePart selectionNotePart;

    private Vector2 selectorPosition;
    private Vector2 ghostNotePosition;
    private bool isHoveringCanvas;

    public bool isPlacingSecondNote = false;

    private bool isSelectingNote;
    private NoteData selectedNote;

    [SerializeField] private int positionSlots = 1000;

    [SerializeField] private Color laneColor = Color.grey;
    [SerializeField] private Color laneOutlineColor = Color.black;
    [SerializeField] private Color beatsPerBarColor = Color.red;
    [SerializeField] private Color divisionColor = Color.black;
    [SerializeField] private Color ghostTapBlockColor = Color.cyan;
    [SerializeField] private Color ghostHoldBlockColor = Color.yellow;
    [SerializeField] private Color tapNoteColor = Color.cyan;
    [SerializeField] private Color holdNoteColor = Color.yellow;
    [SerializeField] private Color rangeColor = new Color(1, 0.92f, 0.016f, 0.5f);

    [MenuItem("Window/Beat Map Editor")]
    public static void OpenWindow()
    {
        var window = GetWindow<BeatMapEditorWindow>();
        window.titleContent = new GUIContent("Beat Map Editor");
        window.Show();
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

    private void OnGUI()
    {
        DrawLeftPanel();
        DrawRightPanel();

        if (GUI.changed && beatmap != null)
        {
            EditorUtility.SetDirty(beatmap);
        }
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
        EditorGUILayout.LabelField(selectionType == EditorSelectionType.Add_Hold_Note ? (isPlacingSecondNote ? "Is Placing 2ND Note" : "Not Placing 2ND Note") : "-");

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Selection Type", EditorStyles.boldLabel);
        selectionType = (EditorSelectionType)GUILayout.SelectionGrid((int)selectionType, Enum.GetNames(typeof(EditorSelectionType)), 1, EditorStyles.radioButton);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Color Settings", EditorStyles.boldLabel);
        laneColor = EditorGUILayout.ColorField("Lane", laneColor);
        laneOutlineColor = EditorGUILayout.ColorField("Lane Outline", laneOutlineColor);
        beatsPerBarColor = EditorGUILayout.ColorField("Beats Per Bar Line", beatsPerBarColor);
        divisionColor = EditorGUILayout.ColorField("Division Line", divisionColor);
        ghostTapBlockColor = EditorGUILayout.ColorField("Ghost Tap Block", ghostTapBlockColor);
        ghostHoldBlockColor = EditorGUILayout.ColorField("Ghost Hold Block", ghostHoldBlockColor);
        tapNoteColor = EditorGUILayout.ColorField("Tap Note", tapNoteColor);
        holdNoteColor = EditorGUILayout.ColorField("Hold Note", holdNoteColor);
        rangeColor = EditorGUILayout.ColorField("Hole Note - Range", rangeColor);

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
                NoteData noteToDelete = GetNoteTapPosition(clickPosition);

                if (noteToDelete == null)
                {
                    if (selectionType == EditorSelectionType.Add_Hold_Note)
                    {
                        noteToDelete = GetHoldNoteFromPosition(clickPosition);
                    }
                }

                if (noteToDelete != null)
                {
                    beatmap.notes.Remove(noteToDelete);
                    Repaint();
                }
            }
        }
        else // Other mouse buttons
        {
            Vector2 clickPosition = e.mousePosition;

            if (selectionType == EditorSelectionType.Selector)
            {
                selectedNote = GetNoteTapPosition(clickPosition);

                if (selectedNote != null)
                {
                    selectionNotePart = EditorSelectionNotePart.TapPosition;
                }
                else if (selectedNote == null)
                {
                    selectedNote = GetHoldNoteFromPosition(clickPosition);
                    selectionNotePart = EditorSelectionNotePart.HoldToPosition;
                }

                isSelectingNote = (selectedNote != null);
            }
            else if (selectionType == EditorSelectionType.Add_Tap_Note && isHoveringCanvas)
            {
                int tapPosition = GetNotePosition(clickPosition);
                int lane = GetNearestLane(clickPosition);
                AddTapNoteToBeatMap(tapPosition, (Lane)lane);
            }
            else if (selectionType == EditorSelectionType.Add_Hold_Note && isHoveringCanvas)
            {
                if (!isPlacingSecondNote)
                {
                    int tapPosition = GetNotePosition(clickPosition);
                    int lane = GetNearestLane(clickPosition);
                    AddHoldNoteStart(tapPosition, (Lane)lane);
                    isPlacingSecondNote = true;
                }
                else
                {
                    int holdToPosition = GetNotePosition(clickPosition);
                    AddHoldNoteEnd(holdToPosition);
                    isPlacingSecondNote = false;
                }
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
                int positionIndex = GetNotePosition(releasePosition);
                int lane = GetNearestLane(releasePosition);
                
                if (selectedNote.type == NoteType.Tap)
                {
                    UpdateNoteTapPosition(selectedNote, positionIndex, (Lane)lane);
                }
                else if (selectedNote.type == NoteType.Hold)
                {
                    if (selectionNotePart == EditorSelectionNotePart.TapPosition)
                    {
                        UpdateNoteTapPosition(selectedNote, positionIndex, (Lane)lane);
                    }
                    else if (selectionNotePart == EditorSelectionNotePart.HoldToPosition)
                    {
                        UpdateNoteHoldToPosition(selectedNote, positionIndex, (Lane)lane);
                    }
                }
            }

            isSelectingNote = false;
            selectedNote = null;
            selectionNotePart = EditorSelectionNotePart.None;
        }
    }

    private void HandleMouseDrag(Event e)
    {
        if (selectionType == EditorSelectionType.Selector && isSelectingNote)
        {
            Vector2 dragPosition = e.mousePosition;
            int positionIndex = GetNotePosition(dragPosition);
            int lane = GetNearestLane(dragPosition);

            if (selectedNote != null)
            {
                if (selectedNote.type == NoteType.Tap)
                {
                    UpdateNoteTapPosition(selectedNote, positionIndex, (Lane)lane);
                }
                else if (selectedNote.type == NoteType.Hold)
                {
                    if (selectionNotePart == EditorSelectionNotePart.TapPosition)
                    {
                        UpdateNoteTapPosition(selectedNote, positionIndex, (Lane)lane);
                    }
                    else if (selectionNotePart == EditorSelectionNotePart.HoldToPosition)
                    {
                        UpdateNoteHoldToPosition(selectedNote, positionIndex, (Lane)lane);
                    }
                }
            }
        }
    }

    private void HandleMouseMove(Event e)
    {
        isHoveringCanvas = beatMapCanvasRect.Contains(e.mousePosition);

        if (selectionType == EditorSelectionType.Add_Tap_Note || selectionType == EditorSelectionType.Add_Hold_Note)
        {
            Vector2 dragPosition = e.mousePosition;
            int tapPosition = GetNotePosition(dragPosition);
            int lane = GetNearestLane(dragPosition);
            ghostNotePosition = GetNotePosition(lane, tapPosition);

            if (selectionType == EditorSelectionType.Add_Hold_Note)
            {
                if (selectedNote != null)
                    selectedNote.holdToPosition = GetNotePosition(e.mousePosition);
            }
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
            if (note.type == NoteType.Tap)
            {
                Vector2 position = GetNotePosition((int)note.lane, note.tapPosition);

                GUIStyle style = new GUIStyle(EditorStyles.textArea);
                style.normal.background = MakeTexture(tapNoteColor);

                GUI.Box(new Rect(position.x, position.y, noteSize.x, noteSize.y), GUIContent.none, style);
            }
            else if (note.type == NoteType.Hold)
            {
                Vector2 tapPosition = GetNotePosition((int)note.lane, note.tapPosition);
                Vector2 holdToPosition = GetNotePosition((int)note.lane, note.holdToPosition);
                float rangeSize = holdToPosition.y - tapPosition.y;

                GUIStyle style = new GUIStyle(EditorStyles.textArea);
                style.normal.background = MakeTexture(rangeColor);

                GUI.Box(new Rect(holdToPosition.x, holdToPosition.y, noteSize.x, -rangeSize), GUIContent.none, style);

                style.normal.background = MakeTexture(holdNoteColor);

                GUI.Box(new Rect(tapPosition.x, tapPosition.y, noteSize.x, noteSize.y), GUIContent.none, style);
                GUI.Box(new Rect(holdToPosition.x, holdToPosition.y, noteSize.x, noteSize.y), GUIContent.none, style);
            }
        }
    }

    private void DrawGhostNote()
    {
        if (selectionType == EditorSelectionType.Add_Tap_Note && isHoveringCanvas)
        {
            GUIStyle ghostStyle = new GUIStyle(EditorStyles.textArea);
            ghostStyle.normal.background = MakeTexture(ghostTapBlockColor);

            GUI.Box(new Rect(ghostNotePosition.x, ghostNotePosition.y, noteSize.x, noteSize.y), GUIContent.none, ghostStyle);
        }
        else if (selectionType == EditorSelectionType.Add_Hold_Note && isHoveringCanvas)
        {
            GUIStyle ghostStyle = new GUIStyle(EditorStyles.textArea);
            ghostStyle.normal.background = MakeTexture(ghostHoldBlockColor);

            GUI.Box(new Rect(ghostNotePosition.x, ghostNotePosition.y, noteSize.x, noteSize.y), GUIContent.none, ghostStyle);
        }
    }

    private void UpdateNoteTapPosition(NoteData note, int tapPosition, Lane lane)
    {
        note.tapPosition = tapPosition;
        note.lane = lane;
    }

    private void UpdateNoteHoldToPosition(NoteData note, int tapPosition, Lane lane)
    {
        note.holdToPosition = tapPosition;
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

    private void AddHoldNoteStart(int tapPosition, Lane lane)
    {
        NoteData note = new NoteData();
        note.type = NoteType.Hold;
        note.lane = lane;
        note.tapPosition = tapPosition;

        beatmap.notes.Add(note);

        selectedNote = note;
    }

    private void AddHoldNoteEnd(int holdToPosition)
    {
        selectedNote.type = NoteType.Hold;

        int currentTapPosition = selectedNote.tapPosition;

        if (holdToPosition < currentTapPosition)
        {
            int tempHold = currentTapPosition;
            currentTapPosition = holdToPosition;
            holdToPosition = tempHold;
        }

        selectedNote.tapPosition = currentTapPosition;
        selectedNote.holdToPosition = holdToPosition;

        selectedNote = null;
    }

    private NoteData GetNoteTapPosition(Vector2 position)
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

    private int GetNotePosition(Vector2 position)
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

    private NoteData GetTapNoteFromPosition(Vector3 clickPosition)
    {
        NoteData noteData = null;

        foreach (NoteData note in beatmap.notes)
        {
            if (note.type == NoteType.Hold && note.tapPosition == GetNotePosition(clickPosition))
            {
                noteData = note;
                break;
            }
        }

        return noteData;
    }

    private NoteData GetHoldNoteFromPosition(Vector3 clickPosition)
    {
        NoteData noteData = null;

        foreach (NoteData note in beatmap.notes)
        {
            if (note.type == NoteType.Hold && note.holdToPosition == GetNotePosition(clickPosition))
            {
                noteData = note;
                break;
            }
        }

        return noteData;
    }

    private Texture2D MakeTexture(Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        return texture;
    }
}