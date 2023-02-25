using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Vector3 = UnityEngine.Vector3;

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
    public AnimationCurve scaleCurve;

    [Header("Audio Mixer Group")]
    public AudioMixerGroup outputMixerGroup;

    [Header("Visualization Settings")]
    [Range(0f, 100f)]
    public float sensitivity = 50f;
    [Range(0f, 100f)]
    public float noiseThreshold = 50f;
    public bool useBuffer = true;
    public bool useScaleCurve = true;

    private List<GameObject> bars;
    private AudioSource audioSource;
    private float[] frequencyRanges = { 20, 50, 150, 250, 450, 650, 1500, 3000, 5000, 20000 };
    private float[] spectrumData;
    private int sampleCount = 8192;
    private int midFreqIndex;
    private float[] barBuffer;
    private float[] frequencyBuffer;
    private float[] smoothBuffer;

    private void Start()
    {
        bars = new List<GameObject>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.outputAudioMixerGroup = outputMixerGroup;
        audioSource.Play();
        spectrumData = new float[sampleCount];
        midFreqIndex = Mathf.FloorToInt(5f / audioClip.frequency * sampleCount / 2);
        barBuffer = new float[numBars];
        frequencyBuffer = new float[sampleCount];
        smoothBuffer = new float[sampleCount];

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

        if (useBuffer)
        {
            Array.Copy(spectrumData, frequencyBuffer, sampleCount);
            Array.Copy(barBuffer, smoothBuffer, numBars);
        }

        for (int i = 0; i < numBars; i++)
        {
            float barHeightSum = 0f;
            int lowFreqIndex = Mathf.FloorToInt((i == 0 ? 0 : frequencyRanges[i - 1]) / audioClip.frequency * sampleCount / 2);
            int highFreqIndex = Mathf.FloorToInt(frequencyRanges[i] / audioClip.frequency * sampleCount / 2);

            for (int j = lowFreqIndex; j <= highFreqIndex; j++)
            {
                float frequency = j * audioClip.frequency / sampleCount;
                float spectrumValue = spectrumData[j];
                float smoothValue = smoothBuffer[i] * 0.9f + spectrumValue * 0.1f;

                if (useBuffer)
                {
                    frequencyBuffer[j] = Mathf.Max(frequencyBuffer[j] - smoothValue * sensitivity, 0f);
                }
                else
                {
                    frequencyBuffer[j] = spectrumValue;
                }

                if (frequencyBuffer[j] > noiseThreshold)
                {
                    float barHeightMultiplier = useScaleCurve ? scaleCurve.Evaluate(frequency / frequencyRanges[i]) : 1f;
                    float barHeightValue = frequencyBuffer[j] * barHeightMultiplier;
                    barHeightSum += barHeightValue;
                }
            }

            float barHeightAverage = barHeightSum / (highFreqIndex - lowFreqIndex + 1);
            float barHeightScaled = Mathf.Lerp(barMinScale, barMaxScale, barHeightAverage / sensitivity * 100);
            barBuffer[i] = barHeightScaled;

            float xPos = (i - (numBars / 2f)) * barDistance;
            Vector3 barScale = new Vector3(barWidth, barHeightScaled, barWidth);
            Vector3 barPosition = new Vector3(xPos, barHeightScaled / 2f, 0f);
            bars[i].transform.localScale = barScale;
            bars[i].transform.localPosition = barPosition;
            Debug.Log($"Bar {i} Position: {barPosition}");
            Debug.Log($"Bar {i} Scale: {barScale}");

        }
    }
}

