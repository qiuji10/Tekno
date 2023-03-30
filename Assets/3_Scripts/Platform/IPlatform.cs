using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlatform
{
    public bool PlayerOnPlatform { get; set; }

    public Transform player { get; set; }
}
