using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

[TrackBindingType(typeof(TextMeshProUGUI))]
[TrackClipType(typeof(SubtitleClip))]
public class SubtitleTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<SubtitleTrackMixer>.Create(graph, inputCount);
    }
}
