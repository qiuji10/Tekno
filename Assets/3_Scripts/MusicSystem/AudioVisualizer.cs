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
    //    // Get data from Koreography
    //    float[] frequencyData = evt.GetValueOfCurveAtTime<float[]>(0);

       
    //    for (int i = 0; i < frequencyData.Length; i++)  // Create bars for each frequency range
    //    {
    //        if (i >= bars.Count)
    //        {
    //            GameObject newBar = Instantiate(barPrefab, transform);
    //            bars.Add(newBar);
    //        }

    //        // Scale bar based on amplitude.
    //        float barScale = Mathf.Clamp(frequencyData[i] * barMaxScale, barMinScale, barMaxScale);
    //        bars[i].transform.localScale = new Vector3(barWidth, barScale, barWidth);

    //        //Position bar and the bar distance
    //        float xPos = (i - (frequencyData.Length / 2f)) * barDistance;
    //        bars[i].transform.localPosition = new Vector3(xPos, barHeight / 2f, 0f);
    //    }

    //    //Disable unused bars.
    //    for (int i = frequencyData.Length; i < bars.Count; i++)
    //    {
    //        bars[i].SetActive(false);
    //    }
    //}
}
