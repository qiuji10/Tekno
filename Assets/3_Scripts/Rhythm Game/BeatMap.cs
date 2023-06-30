using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.Callbacks;
using UnityEditor;
using Unity.VisualScripting;

public enum Division { divisions_4 = 4, divisions_8 = 8, divisions_12 = 12 }
public enum Lane { Lane1, Lane2, Lane3, Lane4 }
public enum NoteType { Tap, Hold }

[CreateAssetMenu(menuName = "Rhythm Game/Beatmap")]
public class BeatMap : ScriptableObject
{
    public int bpm;
    public Vector2Int timeSignature; // x is beat and y is bar
    public Division division;

    public List<NoteData> notes = new List<NoteData>();

    /// <summary>
    /// This function is for getting the first available beat in beatmap, to determine which position on the map to play the audio
    /// </summary>
    /// <returns></returns>
    public int GetHighestTapPosition()
    {
        int highestTapPosition = 0;

        foreach (NoteData note in notes)
        {
            if (note.tapPosition > highestTapPosition)
            {
                highestTapPosition = note.tapPosition;
            }
        }

        return highestTapPosition;
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

}

[Serializable]
public class NoteData
{
    public NoteType type;
    public Lane lane;
    public int tapPosition;
}

[Serializable]
public class Note_Tap : NoteData
{

}

[Serializable]
public class Note_Hold : NoteData
{
    public int holdToPosition;
}