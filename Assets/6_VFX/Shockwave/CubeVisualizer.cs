using SonicBloom.Koreo.Demos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeVisualizer : MonoBehaviour
{
    public AudioSource audioSource;

    public int cubeCount;
    private Material cubeMaterial;
    public GameObject cubePrefab;
    private GameObject[] audioCubes;

    public float rotationalAngle;
    public float cubeSensitivity;

    public float visualiserScale;
    public float cubeScale;
    private float[] audioSamples = new float [128];

    public bool scaleYAxis = false;
    public bool scaleZAxis = false;

    void Start()
    {
        audioCubes = new GameObject[cubeCount];
        CreateCubes();
    }

    void CreateCubes()
    {
        for (int i = 0; i < cubeCount; i++)
        {
            GameObject audioCubesInstance = Instantiate(cubePrefab);
            audioCubesInstance.transform.position = this.transform.position;
            audioCubesInstance.transform.parent = this.transform;
            this.transform.eulerAngles = new Vector3(0, -(rotationalAngle / cubeCount) * i, 0);
            audioCubesInstance.transform.position = Vector3.forward * visualiserScale;
            audioCubes[i] = audioCubesInstance;
        }
    }


    void Update()
    {
        GetAudioSource();
        if (scaleYAxis)
        {
            VisualiseAudioTop();
        }
        if (scaleZAxis)
        {
            VisualiseAudioBot();
        }
    }

    private void GetAudioSource()
    {
        audioSource.GetSpectrumData(audioSamples, 0, FFTWindow.Blackman);
    }

    private void VisualiseAudioTop()
    {
        for (int i = 0; i < cubeCount; i++)
        {
            if(audioCubes != null)
            {
                audioCubes[i].transform.localScale = new Vector3(cubeScale, (audioSamples[i] * cubeSensitivity), 0.01f);
            }
        }
    }

    private void VisualiseAudioBot()
    {
        for (int i = 0; i < cubeCount; i++)
        {
            if (audioCubes != null)
            {
                audioCubes[i].transform.localScale = new Vector3(cubeScale, cubeScale, (audioSamples[i] * cubeSensitivity));
            }
        }
    }
}
