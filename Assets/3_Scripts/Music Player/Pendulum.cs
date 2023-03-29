using UnityEngine;

public class Pendulum : MonoBehaviour
{
    public bool isUsingHook;
    public float speed = 1f;
    public float amplitude = 45f;
    [Range(0f, 1f)] public float startSineRatio = 0f;

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
        angle = amplitude * Mathf.Sin((elapsedTime * speed) + timeOffset); // Use a sine wave with phase offset

        Quaternion rotation = Quaternion.AngleAxis(angle, transform.parent.right);
        Quaternion targetRotation = rotation * transform.parent.rotation;
        rb.MoveRotation(targetRotation);
    }



    public void ResetPendulum()
    {
        elapsedTime = 0;
    }
}
