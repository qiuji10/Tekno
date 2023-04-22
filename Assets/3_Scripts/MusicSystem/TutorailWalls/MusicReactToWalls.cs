using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicReactToWalls : MonoBehaviour
{

    [SerializeField] private Material material;
    [SerializeField] private Material materialStatic;
    [SerializeField] private Material materialStatic1;
    [SerializeField] private Material materialStatic2;

    private void OnEnable()
    {
        StanceManager.OnStanceChangeStart += StanceManager_OnStanceChange;
    }

    private void StanceManager_OnStanceChange(Track obj)
    {
        float speed = 0.0f;
        Color gridColor = Color.clear;

        switch (obj.genre)
        {
            case Genre.House:
                speed = 0.1f;
                gridColor = new Color(1.72079539f, 1.57664502f, 0, 0);
               
                break;

            case Genre.Techno:
                speed = 0.5f;
                gridColor = new Color(0, 0.205526888f, 3.92452836f, 0);
                break;

            case Genre.Electronic:
                speed = 1.5f;
                gridColor = new Color(0.0313725509f, 1.74117649f, 0, 0);
               



                break;

            default:
                speed = 1.0f;
                gridColor = Color.white;
                break;
        }

        // Set the new values for the shader graph parameters
        material.SetFloat("_speed", speed);
        material.SetColor("_GridColor", gridColor);
        materialStatic.SetColor("_GridColor", gridColor);
        materialStatic1.SetColor("_GridColor", gridColor);
        materialStatic2.SetColor("_GridColor", gridColor);
    } 

    private void OnDisable()
    {
        StanceManager.OnStanceChangeStart -= StanceManager_OnStanceChange;
    }
}
