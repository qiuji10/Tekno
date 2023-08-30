using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Beat_UI : MonoBehaviour
{
    private Image img;

    [SerializeField] private TMPro.TMP_Text text;

    private int index;

    private void Awake()
    {
        img = GetComponent<Image>();
    }

    private void OnEnable()
    {
        TempoManager.OnBeat += TempoManager_OnBeat;
    }

    private void OnDisable()
    {
        TempoManager.OnBeat -= TempoManager_OnBeat;
    }

    private void TempoManager_OnBeat()
    {
        string beatText = null;


        index++;
        beatText = index.ToString();

        if (index == 4)
        {
            //beatText = "GO!";
            index = 0;
        }

        text.text = beatText;

        if (img.fillAmount > 0.9f)
        {
            img.fillAmount = 0;
        }

        img.fillAmount += 0.25f;
    }
}
