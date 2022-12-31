using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FloorState { None = 0, Lifting, Dropping }

public class FloorTile : MonoBehaviour
{
    [Header("Lift Y Position")]
    [SerializeField] private float liftTimer = 3f;
    [SerializeField] private AnimationCurve liftCurve;

    [Header("Drop Y Position")]
    [SerializeField] private float dropTimer = 3f;
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
    public void Lift()
    {
        state = FloorState.Lifting;
        StartCoroutine(Lerp_Logic(liftTimer, liftCurve, null));
    }

    [Button]
    public void Drop()
    {
        state = FloorState.Dropping;
        StartCoroutine(Lerp_Logic(dropTimer, dropCurve, Lift));
    }

    IEnumerator Lerp_Logic(float lerpTimer, AnimationCurve curve, Action callback)
    {
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
