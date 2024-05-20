using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using AYellowpaper.SerializedCollections;
using UnityEngine.UI.Extensions;

public class Minigame_Visuals : MonoBehaviour
{
    [System.Serializable]
    public struct InputSprite
    {
        public Sprite sprite;
        public Color color;
    }

    #region UI References
    [Header("[Main UI]")]
    [SerializeField] GameObject panel;
    [SerializeField] TMP_Text countdownText;
    [SerializeField] TMP_Text promptText;

    [Header("[Speaker UI]")]
    [SerializeField] Image speakerImg;
    [SerializeField] UI_Shake speakerHealthBar;
    [SerializeField] List<CustomSlider> speakerSliders = new List<CustomSlider>();

    [Header("[Amplifier UI]")]
    [SerializeField] Image ampCoreImg;
    [SerializeField] UI_Shake amplifierHealthBar;
    [SerializeField] List<CustomSlider> amplifierSliders = new List<CustomSlider>();

    [Header("[Speaker Sprites]")]
    [SerializeField] Sprite speakerReady;
    [SerializeField] Sprite speakerOn;
    [SerializeField] Sprite speakerOff;
    [SerializeField] Sprite speakerSuccess;

    [Header("[Prefabs]")]
    [SerializeField] BeatNote beatNotePrefab;
    [SerializeField] BeatPath beatPathPrefab;

    [Header("[Containers]")]
    [SerializeField] Transform sliderVisualParent;
    [SerializeField] Transform beatVisualParent;

    [Header("[Input Sprites]")]
    [SerializeField] SerializedDictionary<KeyInput, InputSprite> inputSprites = new();

    [Header("Visuals")]
    [SerializeField] private UIParticleSystem successParticle;
    [SerializeField] private UIParticleSystem failParticle;
    #endregion

    #region Health Sliders
    public void SetSpeakerHealth(int curHealth, Action onComplete)
    {
        StartCoroutine(SetSliders(speakerSliders, curHealth, onComplete));
    }

    public void SetAmplifierHealth(int curHealth, Action onComplete)
    {
        StartCoroutine(SetSliders(amplifierSliders, curHealth, onComplete));
    }

    private int GetHealth(List<CustomSlider> sliders)
    {
        float health = 0f;

        for (int i = 0; i < sliders.Count; i++)
        {
            health += sliders[i].Value;
        }

        return Mathf.RoundToInt(health);
    }

    private IEnumerator SetSliders(List<CustomSlider> sliders, int health, Action onComplete)
    {
        float timeToNextBeat = TempoManager.GetTimeToBeatCount(1);
        int prevHealth = GetHealth(sliders);
        float healthDiff = Mathf.Abs(health - prevHealth);
        float lerpTime = timeToNextBeat / healthDiff;

        if (health < prevHealth)
        {
            for (int i = sliders.Count - 1; i >= 0; i--)
            {
                if (sliders[i].Value <= 0)
                    continue;

                bool done = false;

                if (i >= health)
                {
                    sliders[i].Lerp(1, 0, lerpTime, () => done = true);
                    while (!done) yield return null;
                }
                else
                    break;
            }
        }
        else if (health > prevHealth)
        {
            for (int i = 0; i < sliders.Count; i++)
            {
                if (sliders[i].Value >= 1)
                    continue;

                bool done = false;

                if (i <= health)
                {
                    sliders[i].Lerp(0, 1, lerpTime, () => done = true);
                    while (!done) yield return null;
                }
                else
                    break;
            }
        }

        onComplete?.Invoke();
    }
    #endregion

    #region Generate Beat Note
    public void ClearBeatNotes()
    {
        beatVisualParent.DestroyChildrens();
    }

    public GameObject GetBeatNote(BeatData beatData)
    {
        BeatNote beatNote = Instantiate(beatNotePrefab, beatVisualParent);
        beatNote.gameObject.SetActive(false);
        beatNote.SetPosition(beatData.position);
        beatNote.SetSprite(inputSprites[beatData.key].sprite);
        beatNote.SetColor(inputSprites[beatData.key].color);
        return beatNote.gameObject;
    }
    #endregion

    #region Generate Beat Path
    public void ClearBeatPath()
    {
        sliderVisualParent.DestroyChildrens();
    }

    public BeatPath GetBeatPath(Vector2 startPos, Vector2 endPos)
    {
        BeatPath beatPath = Instantiate(beatPathPrefab, sliderVisualParent);
        beatPath.SetPosition(startPos, endPos);
        return beatPath;
    }
    #endregion

    #region Text
    public void EnableCountdown(bool enabled) => countdownText.gameObject.SetActive(enabled);

    public void SetCountdownText(string text) => countdownText.SetText(text);
    public void SetPromptText(string text) => promptText.SetText(text);
    #endregion

    public void OpenPanel(bool enabled) => panel.SetActive(enabled);
}
