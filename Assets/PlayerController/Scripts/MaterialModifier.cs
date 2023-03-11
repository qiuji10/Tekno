using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class MaterialModifier : MonoBehaviour
{
    [SerializeField] List<Material> m_Materials = new List<Material>();
    [SerializeField] private Volume glitchVolume;

    [Header("Settings")]
    [SerializeField] private float glitchFadeIn = 0.25f;
    [SerializeField] private float glitchFadeOut = 0.25f;

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
            while (m_Materials[0].GetFloat(property) < value)
            {
                timer += Time.deltaTime;
                m_Materials[0].SetFloat(property, timer / fadeTime);
                m_Materials[1].SetFloat(property, timer / fadeTime);
                m_Materials[2].SetFloat(property, timer / fadeTime);
                m_Materials[3].SetFloat(property, timer / fadeTime);
                m_Materials[4].SetFloat(property, timer / fadeTime);
                m_Materials[5].SetFloat(property, timer / fadeTime);
                m_Materials[6].SetFloat(property, timer / fadeTime);
                m_Materials[7].SetFloat(property, timer / fadeTime);
                m_Materials[8].SetFloat(property, timer / fadeTime);
                m_Materials[9].SetFloat(property, timer / fadeTime);
                m_Materials[10].SetFloat(property, timer / fadeTime);
                yield return null;
            }
        }
        else
        {
            while (m_Materials[0].GetFloat(property) > value)
            {
                timer -= Time.deltaTime;
                m_Materials[0].SetFloat(property, timer / fadeTime);
                m_Materials[1].SetFloat(property, timer / fadeTime);
                m_Materials[2].SetFloat(property, timer / fadeTime);
                m_Materials[3].SetFloat(property, timer / fadeTime);
                m_Materials[4].SetFloat(property, timer / fadeTime);
                m_Materials[5].SetFloat(property, timer / fadeTime);
                m_Materials[6].SetFloat(property, timer / fadeTime);
                m_Materials[7].SetFloat(property, timer / fadeTime);
                m_Materials[8].SetFloat(property, timer / fadeTime);
                m_Materials[9].SetFloat(property, timer / fadeTime);
                m_Materials[10].SetFloat(property, timer / fadeTime);
                yield return null;
            }
        }

        m_Materials[0].SetFloat(property, value);
        m_Materials[1].SetFloat(property, value);
        m_Materials[2].SetFloat(property, value);
        m_Materials[3].SetFloat(property, value);
        m_Materials[4].SetFloat(property, value);
        m_Materials[5].SetFloat(property, value);
        m_Materials[6].SetFloat(property, value);
        m_Materials[7].SetFloat(property, value);
        m_Materials[8].SetFloat(property, value);
        m_Materials[9].SetFloat(property, value);
        m_Materials[10].SetFloat(property, value);
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
