using UnityEditor;
using UnityEngine;

public class NoiseMaker : MonoBehaviour
{
    public ComputeShader computeShader;
    ComputeBuffer buffer;
    public Material fogMat;
    public int size = 32, height = 16;

    static Texture3D tex3D;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CreateTexture();
        }
    }

    public float noiseSize = 1, seed = 0;

    [ContextMenu("Generate Noise")]
    void CreateTexture()
    {
        if (tex3D == null)
        {
            tex3D = new Texture3D(size, height, size, TextureFormat.RFloat, false);
        }

        fogMat.SetTexture("_Noise", tex3D);
        int pixels = size * size * height;
        ComputeBuffer buffer = new ComputeBuffer(pixels, sizeof(float));

        computeShader.SetBuffer(0, "Result", buffer);
        computeShader.SetFloat("size", size);
        computeShader.SetFloat("height", height);
        computeShader.SetFloat("seed", seed);
        computeShader.SetFloat("noiseSize", noiseSize);
        computeShader.Dispatch(0, size / 8, height / 8, size / 8);

        float[] noise = new float[pixels];
        Color[] colors = new Color[pixels];
        buffer.GetData(noise);
        buffer.Release();

        for (int i = 0; i < pixels; i++)
        {
            colors[i] = new Color(noise[i], 0, 0, 0);
        }
        tex3D.SetPixels(colors);
        tex3D.Apply();

    }


    [ContextMenu("Save Noise")]
    void CreateTexture3D()
    {
        // Save the texture to your Unity Project
        AssetDatabase.CreateAsset(tex3D, "Assets/Volume Fog/3DTexture.asset");
    }
}
