using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    [SerializeField] private PlayerController controller;

    private void Start()
    {
        controller = GetComponent<PlayerController>();  
    }
}
