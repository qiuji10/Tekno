using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using AYellowpaper.SerializedCollections;

[System.Serializable]
public struct StanceColor
{
    [ColorUsage(false, true)] public Color blue;
    [ColorUsage(false, true)] public Color yellow;
    [ColorUsage(false, true)] public Color green;
}

public class RhythmBeatIndicator : MonoBehaviour
{
    [SerializeField] MeshRenderer _renderer;

    [SerializeField] StanceColor stanceColor;

    [SerializeField, Foldout("Colors"), ColorUsage(false, true)] Color blue;
    [SerializeField, Foldout("Colors"), ColorUsage(false, true)] Color yellow;
    [SerializeField, Foldout("Colors"), ColorUsage(false, true)] Color green;

    [SerializeField, ReadOnly] private int curIndex, prevIndex = 3;

    private Dictionary<Genre, Color> colors = new();

    private int count => _renderer.materials.Length;
    private Color white => new Color(2.97355318f, 2.78337145f, 2.39847922f, 1);
    private Genre genre => StanceManager.curTrack.genre;

    private void Awake()
    {
        colors = new()
        {
            { Genre.Electronic, stanceColor.green },
            { Genre.House, stanceColor.yellow },
            { Genre.Techno, stanceColor.blue },
        };
    }

    private void OnEnable()
    {
        TempoManager.OnBeat += OnBeat;
    }

    private void OnDisable()
    {
        TempoManager.OnBeat -= OnBeat;
    }

    private void OnBeat()
    {
        MaterialPropertyBlock property = new();

        _renderer.GetPropertyBlock(property, prevIndex);
        property.SetColor("_TopColor", white);
        _renderer.SetPropertyBlock(property, prevIndex);

        _renderer.GetPropertyBlock(property, curIndex);
        property.SetColor("_TopColor", colors[genre]);
        _renderer.SetPropertyBlock(property, curIndex);

        prevIndex = curIndex;
        curIndex++;

        if (curIndex == count)
            curIndex = 0;
    }
}
