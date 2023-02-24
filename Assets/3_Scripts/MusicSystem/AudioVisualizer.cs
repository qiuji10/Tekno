using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVisualizer : MonoBehaviour
{
    [Header("Audio Clip")]
    public AudioClip audioClip;

    [Header("Bar Properties")]
    public GameObject barPrefab;
    public int numBars = 64;
    public float barDistance = 1f;
    public float barHeight = 10f;
    public float barWidth = 0.2f;
    public float barMinScale = 1f;
    public float barMaxScale = 10f;
    private List<GameObject> bars;
    private AudioSource audioSource;
    private float[] frequencyRanges = { 20, 60, 250, 500, 2000, 4000, 6000, 20000 };
    private float[] spectrumData;
    private int sampleCount = 8192;

    private void Start()
    {
        bars = new List<GameObject>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.Play();
        spectrumData = new float[sampleCount];
        for (int i = 0; i < numBars; i++)
        {
            float lowFreq = i == 0 ? 0 : frequencyRanges[i - 1];
            float highFreq = frequencyRanges[i];
            int lowFreqIndex = Mathf.FloorToInt(lowFreq / audioClip.frequency * sampleCount / 2);
            int highFreqIndex = Mathf.FloorToInt(highFreq / audioClip.frequency * sampleCount / 2);
            GameObject newBar = Instantiate(barPrefab, transform);
            bars.Add(newBar);
            newBar.transform.localScale = new Vector3(barWidth, barMinScale, barWidth);
            float xPos = (i - (numBars / 2f)) * barDistance;
            newBar.transform.localPosition = new Vector3(xPos, barHeight / 2f, 0f);
        }
    }

    private void Update()
    {
        audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.Rectangular);
        for (int i = 0; i < numBars; i++)
        {
            float barHeightSum = 0f;
            int lowFreqIndex = Mathf.FloorToInt((i == 0 ? 0 : frequencyRanges[i - 1]) / audioClip.frequency * sampleCount / 2);
            int highFreqIndex = Mathf.FloorToInt(frequencyRanges[i] / audioClip.frequency * sampleCount / 2);
            for (int j = lowFreqIndex; j <= highFreqIndex; j++)
            {
                barHeightSum += spectrumData[j];
            }
            float barHeightAvg = barHeightSum / (highFreqIndex - lowFreqIndex + 1);
            float barScale = Mathf.Clamp(barHeightAvg * barMaxScale, barMinScale, barMaxScale);
            bars[i].transform.localScale = new Vector3(barWidth, barScale, barWidth);
        }
    }
}
