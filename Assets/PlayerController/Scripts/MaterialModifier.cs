using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Rendering;

public class MaterialModifier : MonoBehaviour
{
    [SerializeField] List<Material> m_Materials = new List<Material>();
    [SerializeField] private Volume glitchVolume;

    [Header("Settings")]
    [SerializeField] private float glitchFadeIn = 0.25f;
    [SerializeField] private float glitchFadeOut = 0.25f;

    [Header("Shirt")]
    [SerializeField, ColorUsage(false, true)] private Color blueHDR;
    [SerializeField, ColorUsage(false, true)] private Color yellowHDR;
    [SerializeField, ColorUsage(false, true)] private Color greenHDR;
    [SerializeField, ColorUsage(false, true)] private Color redHDR;

    [SerializeField] private Texture2D blueTexture;
    [SerializeField] private Texture2D yellowTexture;
    [SerializeField] private Texture2D greenTexture;

    private bool isFading;

    private void OnEnable()
    {
        //string baseString = "#0042FF";
        //string emissionString = "#0054FF";
        //ColorUtility.TryParseHtmlString(baseString, out Color baseColor);
        //ColorUtility.TryParseHtmlString(emissionString, out Color emissionColor);
        //m_Materials[1].SetColor("_BaseColor", baseColor);
        //m_Materials[1].SetColor("_Color", baseColor);
        //m_Materials[1].SetColor("_EmissionColor", emissionColor);
        //m_Materials[2].SetColor("_TextureColor", blueHDR);
        //m_Materials[2].SetTexture("_Texture", blueTexture);

        //StanceManager.OnStanceChangeStart += StanceManager_OnStanceChange;
    }

    private void StanceManager_OnStanceChange(Track track)
    {
        string baseString = null, emissionString = null;

        switch (track.genre)
        {
            case Genre.House:
                baseString = "#988C00";
                emissionString = "#8E7C00";
                break;
            case Genre.Techno:
                baseString = "#0042FF";
                emissionString = "#0054FF";
                break;
            case Genre.Electronic:
                
                baseString = "#009A04";
                emissionString = "#00BC00";
                break;
        }

        ColorUtility.TryParseHtmlString(baseString, out Color baseColor);
        ColorUtility.TryParseHtmlString(emissionString, out Color emissionColor);
        m_Materials[1].SetColor("_BaseColor", baseColor);
        m_Materials[1].SetColor("_EmissionColor", emissionColor);
    }

    private void OnDisable()
    {
       // StanceManager.OnStanceChangeStart -= StanceManager_OnStanceChange;
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

    public void DeathMaterial()
    {
        m_Materials[0].SetColor("_Emission", redHDR);
        m_Materials[1].SetColor("_Emission", redHDR);
        m_Materials[2].SetColor("_Emission", redHDR);

        string property = "_GlitchPower";
        m_Materials[0].SetFloat(property, 1);
        m_Materials[1].SetFloat(property, 1);
        m_Materials[2].SetFloat(property, 1);
    }

    public void ResetMaterial()
    {
        string property = "_GlitchPower";
        m_Materials[0].SetFloat(property, 0);
        m_Materials[1].SetFloat(property, 0);
        m_Materials[2].SetFloat(property, 0);
    }

    public void StanceChange()
    {
        string baseString = null, emissionString = null;
        Texture tex = null;
        Color texColor = Color.white;

        switch (StanceManager.curTrack.genre)
        {
            case Genre.House:
                baseString = "#988C00";
                emissionString = "#8E7C00";
                tex = yellowTexture;
                texColor = yellowHDR;
                break;
            case Genre.Techno:
                baseString = "#0042FF";
                emissionString = "#0054FF";
                tex = blueTexture;
                texColor = blueHDR;
                break;
            case Genre.Electronic:

                baseString = "#009A04";
                emissionString = "#00BC00";
                tex = greenTexture;
                texColor = greenHDR;
                break;
        }

        ColorUtility.TryParseHtmlString(baseString, out Color baseColor);
        ColorUtility.TryParseHtmlString(emissionString, out Color emissionColor);
        m_Materials[1].SetColor("_BaseColor", baseColor);
        m_Materials[1].SetColor("_Emission", emissionColor);

        m_Materials[2].SetColor("_TextureColor", texColor);
        m_Materials[2].SetTexture("_Texture", tex);
    }

    IEnumerator ShaderFader(string property, float fadeTime, float value, bool increment)
    {
        float timer = 0;

        if (increment)
        {
            while (m_Materials[0].GetFloat(property) < value)
            {
                timer += Time.deltaTime;
                float ratio = Mathf.Clamp(timer / fadeTime, 0f, 1f);
                m_Materials[0].SetFloat(property, ratio);
                m_Materials[1].SetFloat(property, ratio);
                m_Materials[2].SetFloat(property, ratio);
                yield return null;
            }
        }
        else
        {
            while (m_Materials[0].GetFloat(property) > value)
            {
                timer -= Time.deltaTime;
                float ratio = Mathf.Clamp(timer / fadeTime, 0f, 1f);
                m_Materials[0].SetFloat(property, ratio);
                m_Materials[1].SetFloat(property, ratio);
                m_Materials[2].SetFloat(property, ratio);
                yield return null;
            }
        }

        m_Materials[0].SetFloat(property, value);
        m_Materials[1].SetFloat(property, value);
        m_Materials[2].SetFloat(property, value);
    }

    IEnumerator VolumeFader(Volume volume, float fadeTime, float value, bool increment)
    {
        if (isFading) yield break;

        isFading = true;

        float timer = 0;
        float initialValue = volume.weight;

        if (increment)
        {
            while (volume.weight < value)
            {
                timer += Time.deltaTime;
                volume.weight = Mathf.Clamp(initialValue + (timer / fadeTime) * (value - initialValue), 0f, 1f);
                yield return null;
            }
        }
        else
        {
            while (volume.weight > value)
            {
                timer += Time.deltaTime;
                volume.weight = Mathf.Clamp(initialValue - (timer / fadeTime) * (initialValue - value), 0f, 1f);
                yield return null;
            }
        }

        volume.weight = value;

        isFading = false;
    }

}
