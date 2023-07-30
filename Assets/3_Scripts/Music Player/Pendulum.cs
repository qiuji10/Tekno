using UnityEngine;

public class Pendulum : MonoBehaviour
{
    public bool isUsingHook;
    public float speed = 1f;
    public float amplitude = 45f;
    [Range(-1f, 1f)] public float startSineRatio = 0f;
    public float lerpRatio;
    public bool isForward;

    private float angle = 0f;
    private float elapsedTime;
    private Rigidbody rb;



    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!isUsingHook) return;

        elapsedTime += Time.deltaTime;

        float timeOffset = startSineRatio * Mathf.PI * 2f; // Convert startRatio to a phase offset
        float sine = Mathf.Sin((elapsedTime * speed) + timeOffset);
        angle = amplitude * sine;// Use a sine wave with phase offset

        lerpRatio = MapSineTo01(angle, amplitude, -amplitude);
        Quaternion rotation = Quaternion.AngleAxis(angle, transform.parent.right);
        Quaternion targetRotation = rotation * transform.parent.rotation;
        rb.MoveRotation(targetRotation);



        if (lerpRatio >= 0.5f)
        {
            isForward = true;
        }
        else
        {
            isForward = false;
        }

        Debug.Log(lerpRatio);
    }

    public void ResetPendulum(float startRatio)
    {
        elapsedTime = 0;
        startSineRatio = startRatio;

    }

    public void ResetPendulum()
    {
        elapsedTime = 0;
        
    }

    private float MapSineTo01(float value, float minValue, float maxValue)
    {
        float normalizedValue = Mathf.InverseLerp(minValue, maxValue, value);
        return normalizedValue;
    }
}
