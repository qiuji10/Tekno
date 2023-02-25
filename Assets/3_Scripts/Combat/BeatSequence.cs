using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBeatSequemce", menuName = "Beat System/New Beat Sequemce")]
public class BeatSequence : ScriptableObject
{
    public List<BeatSettings> beatSettings = new List<BeatSettings>();
}
