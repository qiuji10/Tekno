using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using UnityEngine;

public class RhythmVisual : MonoBehaviour
{
    [SerializeField] private int index = 1;
    [SerializeField] private string colorKeyword = "_Main_Tint";
    
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

        private void OnDisable()
    {
        StanceManager.OnStanceChangeStart -= ChangeMaterialColor;
    }

    private void ChangeMaterialColor(Track track)
    {
        MaterialPropertyBlock property = new();

        if (index < 0)
            _renderer.GetPropertyBlock(property);
        else
            _renderer.GetPropertyBlock(property, index);

        property.SetColor(colorKeyword, colors[track.genre]);

        if (index < 0)
            _renderer.GetPropertyBlock(property);
        else
            _renderer.SetPropertyBlock(property, index);
    }
}
