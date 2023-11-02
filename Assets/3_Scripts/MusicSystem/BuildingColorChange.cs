using NaughtyAttributes;
using SonicBloom.Koreo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingColorChange : MonoBehaviour
{
    [SerializeField]
    private int materialIndex = 0;
    [SerializeField] Material[] genreMaterials;

    private void Awake()
    {
        StartCoroutine(ColorChangeDelay(1, 0f));
    }

    private void OnEnable()
    {
        StanceManager.OnStanceChangeStart += StanceManager_OnStanceChange;
    }

    private void OnDisable()
    {
        StanceManager.OnStanceChangeStart -= StanceManager_OnStanceChange;
    }

    private void StanceManager_OnStanceChange(Track obj)
    {
        int specificMaterialIndex = GetSpecificMaterialIndex(obj.genre);
        StartCoroutine(ColorChangeDelay(specificMaterialIndex, 1.65f));
    }

    private int GetSpecificMaterialIndex(Genre genre)
    {
        switch (genre)
        {
            case Genre.House:
                return 0;
            case Genre.Techno:
                return 1;
            case Genre.Electronic:
                return 2;
            default:
                return 1;
        }
    }

    private IEnumerator ColorChangeDelay(int specificMaterialIndex, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        Renderer[] childRenderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in childRenderers)
        {
            if (materialIndex < renderer.sharedMaterials.Length)
            {
                Material[] newMaterials = renderer.sharedMaterials;
                newMaterials[materialIndex] = genreMaterials[specificMaterialIndex];
                renderer.sharedMaterials = newMaterials;
            }
        }
    }
}
