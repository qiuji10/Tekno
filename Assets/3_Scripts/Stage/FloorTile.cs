using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FloorState { None = 0, Lifting, Dropping }

public class FloorTile : MonoBehaviour
{
    [Header("Lift Y Position")]
    public float liftTimer = 3f;
    [SerializeField] private AnimationCurve liftCurve;

    [Header("Drop Y Position")]
    public float dropTimer = 3f;
    [SerializeField] private AnimationCurve dropCurve;

    private FloorState state = FloorState.None;
    public FloorState State { get { return state; } }
    private float _timer = 0f;

    private MeshRenderer _mesh;

    private void Awake()
    {
        _mesh = GetComponent<MeshRenderer>();
    }

    [Button]
    public void Drop()
    {
        state = FloorState.Dropping;
        StartCoroutine(Lerp_Logic(0, dropTimer, dropCurve, Lift));
    }

    [Button]
    public void Lift()
    {
        state = FloorState.Lifting;
        StartCoroutine(Lerp_Logic(0, liftTimer, liftCurve, null));
    }

    public void DropWithAlert(float delay)
    {
        state = FloorState.Dropping;
        StartCoroutine(Lerp_Logic(delay, dropTimer, dropCurve, LiftFromAlertDrop));
    }

    public void LiftFromAlertDrop()
    {
        state = FloorState.Lifting;
        StartCoroutine(Lerp_Logic(0, liftTimer, liftCurve, null));
    }

    IEnumerator Lerp_Logic(float delay, float lerpTimer, AnimationCurve curve, Action callback)
    {
        yield return new WaitForSeconds(delay);

        float finalYPos = 0;
        while (_timer < lerpTimer)
        {
            _timer += Time.deltaTime;
            float lerpRatio = _timer / lerpTimer;
            float yPos = finalYPos = curve.Evaluate(lerpRatio);
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, yPos, transform.position.z), lerpRatio);
            yield return null;
        }

        int yPosInt = Mathf.RoundToInt(finalYPos);
        transform.position = new Vector3(transform.position.x, yPosInt, transform.position.z);
        _timer = 0;
        state = FloorState.None;

        if (callback != null)
        {
            callback();
        }
    }

    public void SetMat(Material newMat)
    {
        _mesh.material = newMat;
    }
}
