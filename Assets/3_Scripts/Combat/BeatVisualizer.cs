using UnityEngine;
using UnityEngine.UI;

public class BeatVisualizer : MonoBehaviour
{
    public float beatSize = 100.0f;
    public float growTime = 0.2f;
    public float shrinkTime = 0.4f;

    private Image image;
    private float originalSize;
    private float targetSize;
    private bool isGrowing;

    void Start()
    {
        image = GetComponent<Image>();
        originalSize = image.rectTransform.sizeDelta.x;
        targetSize = originalSize;
        isGrowing = false;
    }

    void OnEnable()
    {
        TempoManager.OnBeat += OnBeat;
    }

    void OnDisable()
    {
        TempoManager.OnBeat -= OnBeat;
    }

    void Update()
    {
        if (isGrowing)
        {
            image.rectTransform.sizeDelta = new Vector2(Mathf.Lerp(image.rectTransform.sizeDelta.x, targetSize, Time.deltaTime / growTime), Mathf.Lerp(image.rectTransform.sizeDelta.y, targetSize, Time.deltaTime / growTime));
        }
        else
        {
            image.rectTransform.sizeDelta = new Vector2(Mathf.Lerp(image.rectTransform.sizeDelta.x, originalSize, Time.deltaTime / shrinkTime), Mathf.Lerp(image.rectTransform.sizeDelta.y, originalSize, Time.deltaTime / shrinkTime));
        }
    }

    private void OnBeat()
    {
       // targetSize = beatSize;

        image.rectTransform.sizeDelta = new Vector2(200, 200);
        isGrowing = true;
    }
}
