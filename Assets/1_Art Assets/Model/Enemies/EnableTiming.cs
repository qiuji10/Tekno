using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableTiming : MonoBehaviour
{
    public List<GameObject> objects = new List<GameObject>();

    private void Start()
    {
        StartCoroutine(WaitAndEnable());
    }

    private IEnumerator WaitAndEnable()
    {
        foreach (GameObject obj in objects)
        {
            obj.SetActive(true);
            float randomTime = Random.Range(0.05f, 1.0f);
            yield return new WaitForSeconds(randomTime);
        }
    }
}
