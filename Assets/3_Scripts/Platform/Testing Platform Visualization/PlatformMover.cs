using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    [SerializeField] private List<Transform> points = new List<Transform>();
    [SerializeField] private int beatCountToTrigger = 3, curBeatCount = 1, index;

    [SerializeField] private Material defaultMaterial;

    [SerializeField] bool test;

    private MeshRenderer _mesh;
    private Material[] materials;

    private void Awake()
    {
        //_mesh = GetComponent<MeshRenderer>();

       // materials = _mesh.materials;

        TempoManager.OnBeat += TempoManager_OnBeat;
    }

    private void OnDestroy()
    {
        TempoManager.OnBeat -= TempoManager_OnBeat;
    }

    private void TempoManager_OnBeat()
    {
        //if (test)
        //{
        //    Material[] newMats = new Material[materials.Length];

        //    if (TempoManager.beatCount == 1)
        //    {
        //        newMats[0] = materials[0];
        //        newMats[1] = materials[1];
        //        newMats[2] = defaultMaterial;
        //        newMats[3] = defaultMaterial;
        //        newMats[4] = defaultMaterial;

        //        _mesh.materials = newMats;
        //    }
        //    else if (TempoManager.beatCount == 2)
        //    {
        //        newMats[0] = materials[0];
        //        newMats[1] = materials[1];
        //        newMats[2] = materials[2];
        //        newMats[3] = defaultMaterial;
        //        newMats[4] = defaultMaterial;
        //        _mesh.materials = newMats;
        //    }
        //    else if (TempoManager.beatCount == 3)
        //    {
        //        newMats[0] = materials[0];
        //        newMats[1] = materials[1];
        //        newMats[2] = materials[2];
        //        newMats[3] = materials[3];
        //        newMats[4] = defaultMaterial;
        //        _mesh.materials = newMats;
        //    }
        //    else if (TempoManager.beatCount == 4)
        //    {
        //        newMats = materials;
        //        _mesh.materials = newMats;
        //    }
        //}

        if (TempoManager.beatCount == 4)
        {
            StartCoroutine(MoveLogic());
        }
    }

    private IEnumerator MoveLogic()
    {
        Vector3 oldPos = transform.position;
        Vector3 newPos = points[index].position;

        float timer = 0f;
        float time = (60f / TempoManager.staticBPM) * 1f;

        while (timer < time)
        {
            float ratio = Mathf.SmoothStep(0f, 1f, timer / time);
            transform.position = Vector3.Lerp(oldPos, newPos, ratio);
            timer += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        index++;

        if (index == points.Count)
            index = 0;

        transform.position = newPos;
    }
}
