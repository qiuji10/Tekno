using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using UnityEngine;

public class RhythmVisual : MonoBehaviour
{
    [SerializeField] MeshRenderer _renderer;
    [SerializeField] StanceColor stanceColor;

    private Dictionary<Genre, Color> colors = new();

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
        StanceManager.OnStanceChangeStart += ChangeMaterialColor;
    }

    private void ChangeMaterialColor(Track track)
    {
        MaterialPropertyBlock property = new();

        _renderer.GetPropertyBlock(property, 1);
        property.SetColor("_Main_Tint", colors[track.genre]);
        _renderer.SetPropertyBlock(property, 1);
    }
}
