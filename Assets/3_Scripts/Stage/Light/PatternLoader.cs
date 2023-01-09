using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class PatternLoader : MonoBehaviour
{
    [SerializeField] SpotLightPattern pattern;

    [SerializeField] List<SplineContainer> container;

    [Button]
    private void Load()
    {
        for (int i = 0; i < container.Count; i++)
        {
            container[i].Spline.Resize(pattern.splines[i].ToArray().Length);
            container[i].Spline = pattern.splines[i];
        }
        
    }
}
