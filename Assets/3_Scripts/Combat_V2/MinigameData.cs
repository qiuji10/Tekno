using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MinigameData : MonoBehaviour
{
    public static MinigameData Instance;

    [Header("Main")]
    public Canvas canvas;
    public Image ampCoreImg;
    public TMP_Text countdownText;
    public TMP_Text promptText;

    [Header("Beat Point Reference")]
    public RectTransform sliderVisualParent;
    public RectTransform beatVisualParent;

    [Header("Health Bar")]
    public RectTransform speakerHealthBar;
    public RectTransform ampHealthBar;

    [Header("SP3AKER")]
    public Minigame_Speaker speaker;
    public Sprite speakerReady;
    public Sprite speakerOn;
    public Sprite speakerOff;
    public Sprite speakerSuccess;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }
}
