using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using AYellowpaper.SerializedCollections;
using UnityEngine.UI.Extensions;
using static Minigame;

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
    [SerializeField] SerializedDictionary<SpeakerStatus, Sprite> speakerStates = new();

    [Header("[Prefabs]")]
    [SerializeField] BeatNote beatNotePrefab;
    [SerializeField] BeatPath beatPathPrefab;

    [Header("[Containers]")]
    [SerializeField] Transform sliderVisualParent;
    [SerializeField] Transform beatVisualParent;

    [Header("[Input Sprites]")]
    [SerializeField] SerializedDictionary<KeyInput, InputSprite> inputSprites = new();

    [Header("[Visuals]")]
    [SerializeField] private UIParticleSystem successParticle;
    [SerializeField] private UIParticleSystem failParticle;
    [SerializeField] private UIParticleSystem arrowPrefab;
    private UIParticleSystem arrowRect;
    #endregion

    private void Awake()
    {
        arrowRect = Instantiate(arrowPrefab, panel.transform);
    }

    public void OpenPanel(bool enabled) => panel.SetActive(enabled);

    #region Health Sliders
    public void ShakeSpeakerHP() => speakerHealthBar.Shake();

    public void ShakeAmplifierHP() => amplifierHealthBar.Shake();

    public void SetSpeakerHP(int curHealth, Action onComplete = null)
    {
        StartCoroutine(SetSliders(speakerSliders, curHealth, onComplete));
    }

    public void SetAmplifierHP(int curHealth, Action onComplete = null)
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
        float timeToNextBeat = 60f / TempoManager.staticBPM;
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

    public BeatNote GetBeatNote(BeatData beatData)
    {
        BeatNote beatNote = Instantiate(beatNotePrefab, beatVisualParent);
        beatNote.gameObject.SetActive(false);
        beatNote.SetPosition(beatData.position);
        beatNote.SetSprite(inputSprites[beatData.key].sprite);
        beatNote.SetColor(inputSprites[beatData.key].color);
        return beatNote;
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

    public void SetPromptText(string text)
    {
        promptText.color = Color.blue;
        promptText.SetText(text);
    }

    public void SetPromptText(string text, Color color)
    {
        promptText.color = color;
        promptText.SetText(text);
    }
    #endregion

    #region VFX
    public void PlaySucessVFX()
    {
        successParticle.StartParticleEmission();
    }

    public void PlayFailVFX()
    {
        failParticle.StartParticleEmission();
    }

    public void LerpArrow(Vector2 from, Vector2 to, float duration)
    {
        arrowRect.StartParticleEmission();

        Vector2 dir = (to - from).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        var rect = arrowRect.transform as RectTransform;
        rect.eulerAngles = new Vector3(0, 0, angle);
        //
        StartCoroutine(Lerp());

        IEnumerator Lerp()
        {
            float timer = 0;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                rect.anchoredPosition = Vector2.Lerp(from, to, timer / duration);
                yield return null;
            }
        }//
    }
    #endregion

    public void SetSpeakerImg(SpeakerStatus status)
    {
        speakerImg.sprite = speakerStates[status];
    }
}
