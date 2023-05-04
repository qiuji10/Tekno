using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpectrumUI : MonoBehaviour
{
    [Header("Value")]
    [Range(0.0000001f, 0.1f)] public float refValue = 0.0027f;
    [Range(0.1f, 5f)] public float scaleMultiplier = 0.75f;
    [Range(0.01f, 1f)] public float releaseTime = 0.2f;
    public float innerRadus = 100f;
    public int elementCount = 92;
    public Vector2 offset;

    [Header("Reference")]
    public AudioSource audioSource;
    public SpectrumElement_UI spectrumElementPrefab;
    public Image teknoImg;
    public List<Image> images = new List<Image>();

    private SpectrumElement_UI[] spectrumElements;
    private float[] spectrum = new float[2048];

    private void Awake()
    {
        if (!audioSource) audioSource = FindObjectOfType<StanceManager>().GetComponent<AudioSource>();
        CreateElements();
    }

    private void OnEnable()
    {
        TempoManager.OnBeat += OnBeat;
    }

    private void OnDisable()
    {
        TempoManager.OnBeat -= OnBeat;
    }

    private void OnBeat()
    {
        scaleMultiplier = 0.75f;
    }

    private void Update()
    {
        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);

        for (int i = 0; i < spectrumElements.Length; i++)
        {
            var value = 20f * Mathf.Log10(spectrum[i + 2] / refValue);
            spectrumElements[i].SetScale(value);
        }

        if (scaleMultiplier > 0.2f)
        {
            scaleMultiplier -= Time.deltaTime * 2;
        }
    }

    private void CreateElements()
    {
        spectrumElements = new SpectrumElement_UI[elementCount];

        for (int i = 0; i < spectrumElements.Length; i++)
        {
            spectrumElements[i] = Instantiate(spectrumElementPrefab, transform, false);
            images.Add(spectrumElements[i].GetComponent<Image>());
        }

        AlignElementsCircularElements();
    }

    private void AlignElementsCircularElements()
    {
        var angleStep = 360f / spectrumElements.Length;

        int angleMultiplier = 0;

        for (int i = 0; i < spectrumElements.Length; i += 2)
        {
            spectrumElements[i].transform.localRotation = Quaternion.identity;
            spectrumElements[i + 1].transform.localRotation = Quaternion.identity;

            //Left
            spectrumElements[i].transform.Rotate(Vector3.forward, angleMultiplier * -angleStep);
            spectrumElements[i].rect.anchoredPosition = spectrumElements[i].transform.up + spectrumElements[i].transform.up * innerRadus + new Vector3(offset.x, offset.y, 0);

            //Right
            spectrumElements[i + 1].transform.Rotate(Vector3.forward, (angleMultiplier + 1) * angleStep);
            spectrumElements[i + 1].rect.anchoredPosition = spectrumElements[i + 1].transform.up + spectrumElements[i + 1].transform.up * innerRadus + new Vector3(offset.x, offset.y, 0);

            angleMultiplier++;
        }

        images.Sort((a, b) => a.rectTransform.localRotation.eulerAngles.z.CompareTo(b.rectTransform.localRotation.eulerAngles.z));
    }
}
