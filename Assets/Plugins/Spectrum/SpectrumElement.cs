using UnityEngine;

public class SpectrumElement : MonoBehaviour
{
    private float _peakScale;
    private float _currentScale;
    private float _lastUpdate;

    private Spectrum spectrum;

    private Mesh mesh;
    private Vector3[] originalMeshVerts;
    private Vector3[] meshVerts;
    private Bounds originalMeshBounds;
    private bool useMeshScaling = true;

    private void Awake()
    {
        spectrum = FindObjectOfType<Spectrum>();

        mesh = GetComponent<MeshFilter>().mesh;
        originalMeshVerts = mesh.vertices;
        originalMeshBounds = mesh.bounds;
        meshVerts = new Vector3[mesh.vertices.Length];
    }

    public void SetScale(float scale)
    {
        if (scale > _currentScale)
        {
            _peakScale = _currentScale = scale;
            _lastUpdate = Time.unscaledTime;
        }
    }

    private void Update()
    {
        var t = (Time.unscaledTime - _lastUpdate) / spectrum.releaseTime;
        _currentScale = Mathf.Lerp(_peakScale, 0f, t);

        if (Input.GetKeyDown(KeyCode.M))
        {
            useMeshScaling = !useMeshScaling;
            if (!useMeshScaling)
            {
                mesh.vertices = originalMeshVerts;
                mesh.RecalculateNormals();
            }
            else
            {
                transform.localScale = Vector3.one;
            }
        }

        if (useMeshScaling)
        {
            UpdateMeshScale(_currentScale * spectrum.scaleMultiplier);
        }
        else
        {
            UpdateTransformScale(Mathf.Clamp(_currentScale * spectrum.scaleMultiplier, 1f, float.MaxValue));
        }
    }

    private void UpdateTransformScale(float scale)
    {
        transform.localScale = new Vector3(1f, scale, 1f);
    }

    private void UpdateMeshScale(float scale)
    {
        var threshold = originalMeshBounds.center.y;

        for (int i = 0; i < originalMeshVerts.Length; i++)
        {
            var vector = originalMeshVerts[i];
            if (vector.y >= threshold)
            {
                meshVerts[i] = new Vector3(vector.x, vector.y + scale, vector.z);
            }
            else
            {
                meshVerts[i] = vector;
            }
        }

        mesh.vertices = meshVerts;
        mesh.RecalculateNormals();
    }
}
