using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBase : MonoBehaviour, IPlatform
{
    public bool PlayerOnPlatform { get; set; }
    public Transform player { get; set; }

}
