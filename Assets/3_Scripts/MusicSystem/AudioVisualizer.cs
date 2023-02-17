using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

public class AudioVisualizer : MonoBehaviour
{
    [Header("Selected Beat Data")]
    [EventID]
    public string eventID;

    [Header("Bar Properties")]
    public GameObject barPrefab;
    public float barDistance = 1f;
    public float barHeight = 10f;
    public float barWidth = 0.2f;
    public float barMinScale = 1f;
    public float barMaxScale = 10f;

    private List<GameObject> bars;

    //void Start()
    //{
    //    bars = new List<GameObject>();
    //    Koreographer.Instance.RegisterForEvents(eventID, OnBeatEvent);
    //}
    //private void OnBeatEvent(KoreographyEvent evt)
    //{

    //    float[] frequencyData = evt.GetValueOfCurveAtTime<float[]>(0);
    //    for (int i = 0; i < frequencyData.Length; i++)
    //    {
    //        if (i >= bars.Count)
    //        {
    //            GameObject newBar = Instantiate(barPrefab, transform);
    //            bars.Add(newBar);
    //        }
    //        float barScale = Mathf.Clamp(frequencyData[i] * barMaxScale, barMinScale, barMaxScale);
    //        bars[i].transform.localScale = new Vector3(barWidth, barScale, barWidth);
    //        float xPos = (i - (frequencyData.Length / 2f)) * barDistance;
    //        bars[i].transform.localPosition = new Vector3(xPos, barHeight / 2f, 0f);
    //    }
    //    for (int i = frequencyData.Length; i < bars.Count; i++)
    //    {
    //        bars[i].SetActive(false);
    //    }
    //}
}
