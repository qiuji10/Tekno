using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

[CreateAssetMenu(menuName = "Spline Data")]
public class SpotLightPattern : ScriptableObject
{
    public List<Spline> splines;
}
