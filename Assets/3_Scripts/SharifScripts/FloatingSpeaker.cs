using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingSpeaker : MonoBehaviour
{
    public Transform player;
    public Transform waypoint;
    public float moveSpeed = 10f;
    public float smoothTimeXZ = 0f;
    public float smoothTimeY = 0.2f;
    public float beatOffsetY = 5f;
    public float rotationSpeed = 10f; // added variable for look at smoothing

    [SerializeField] private List<MeshRenderer> speakerVisuals = new List<MeshRenderer>();

    private Vector3 velocityXZ = Vector3.zero;
    private Vector3 velocityY = Vector3.zero;
    private Vector3 targetPosition;
    private bool isOnBeat = false;

    private void OnEnable()
    {
        TempoManager.OnBeat += TempoManager_OnBeat;
        StanceManager.OnStanceChangeStart += StanceManager_OnStanceChangeStart;
    }

    private void OnDisable()
    {
        TempoManager.OnBeat -= TempoManager_OnBeat;
        StanceManager.OnStanceChangeStart -= StanceManager_OnStanceChangeStart;
    }

    private void StanceManager_OnStanceChangeStart(Track obj)
    {
        foreach (var visual in speakerVisuals)
        {
            visual.enabled = false;
        }

        StartCoroutine(EnableVisuals());
    }

    private void TempoManager_OnBeat()
    {
        isOnBeat = true;
    }

    IEnumerator EnableVisuals()
    {
        yield return new WaitForSeconds(2.333f);
        foreach (var visual in speakerVisuals)
        {
            visual.enabled = true;
        }
    }

    void FixedUpdate()
    {
        targetPosition = waypoint.position;
        Vector3 targetPositionXZ = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        transform.position = Vector3.SmoothDamp(transform.position, targetPositionXZ, ref velocityXZ, smoothTimeXZ);

        Vector3 targetPositionY = new Vector3(transform.position.x, targetPosition.y, transform.position.z);

        if (isOnBeat)
        {
            targetPositionY.y -= beatOffsetY;
            isOnBeat = false;
        }

        //transform.position = Vector3.SmoothDamp(transform.position, targetPositionY, ref velocityY, smoothTimeY);
        //Quaternion targetRotation = Quaternion.LookRotation(player.position);
        //transform.rotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f);

        transform.position = Vector3.SmoothDamp(transform.position, targetPositionY, ref velocityY, smoothTimeY);
        Quaternion targetRotation = Quaternion.LookRotation(player.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f), Time.deltaTime * rotationSpeed); // apply rotation speed adjustment
    }
}
