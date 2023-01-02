using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MaterialModifier : MonoBehaviour
{
    [SerializeField] private Renderer m_Renderer;
    [SerializeField] private Volume glitchVolume;

    [Header("Settings")]
    [SerializeField] private float glitchFadeIn = 0.25f;
    [SerializeField] private float glitchFadeOut = 0.25f;

    private Material m_Material;

    private void Awake()
    {
        m_Material = m_Renderer.material;
    }

    public void StopCoroutines()
    {
        StopAllCoroutines();
    }

    [Button]
    public void GlitchyEffectOn()
    {
        StartCoroutine(ShaderFader("_GlitchPower", glitchFadeIn, 1f, true));
        StartCoroutine(VolumeFader(glitchVolume, glitchFadeIn, 1f, true));
    }

    [Button]
    public void GlitchyEffectOff()
    {
        StartCoroutine(ShaderFader("_GlitchPower", glitchFadeOut, 0f, false));
        StartCoroutine(VolumeFader(glitchVolume, glitchFadeOut, 0f, false));
    }

    IEnumerator ShaderFader(string property, float fadeTime, float value, bool increment)
    {
        float timer = 0;

        if (increment)
        {
            while (m_Material.GetFloat(property) < value)
            {
                timer += Time.deltaTime;
                m_Material.SetFloat(property, timer / fadeTime);
                yield return null;
            }
        }
        else
        {
            timer = m_Material.GetFloat(property);

            while (m_Material.GetFloat(property) > value)
            {
                timer -= Time.deltaTime;
                m_Material.SetFloat(property, timer / fadeTime);
                yield return null;
            }
        }

        m_Material.SetFloat(property, value);
    }

    IEnumerator VolumeFader(Volume volume, float fadeTime, float value, bool increment)
    {
        float timer = 0;

        if (increment)
        {
            while (volume.weight < value)
            {
                timer += Time.deltaTime;
                volume.weight = timer / fadeTime;
                yield return null;
            }
        }
        else
        {
            timer = volume.weight;

            while (volume.weight > value)
            {
                timer -= Time.deltaTime;
                volume.weight = timer / fadeTime;
                yield return null;
            }
        }

        volume.weight = value;
    }
}
