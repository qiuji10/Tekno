using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundWave : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public int pointCount;
    public float maxRadius;
    public float speed;
    public float startWidth;
    [SerializeField] private Material material;

    //private void OnEnable()
    //{
    //    StanceManager.OnStanceChange += StanceManager_OnStanceChange;
    //}

    //private void OnDisable()
    //{
    //    StanceManager.OnStanceChange -= StanceManager_OnStanceChange;
    //}

    //private void StanceManager_OnStanceChange(Track obj)
    //{
    //    switch (obj.genre)
    //    {
    //        case Genre.Techno:
    //            material.color = new Color(0.259434f, 0.366f, 1);
    //            break;
    //        case Genre.Electronic:
    //            material.color = new Color(0.3271575f, 1, 0.2588235f);
    //            break;
    //        case Genre.House:
    //            material.color = new Color(1, 0.9328827f, 0.2588235f);
    //            break;
    //    }
    //}
    private void Awake()
    {
        lineRenderer= GetComponent<LineRenderer>();
    }

    public IEnumerator Wave()
    {
        lineRenderer.positionCount = pointCount + 1;
        float currentRadius = 0f;

        while(currentRadius < maxRadius)
        {
            currentRadius += Time.deltaTime * speed;
            Draw(currentRadius);
            yield return null;
        }
        
    }

    public void Draw(float currentRadius)
    {
        float angleBetweenPoints = 360f / pointCount;

        for(int i = 0; i <= pointCount; i++)
        {
            float angle = i * angleBetweenPoints * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0f);
            Vector3 position = direction* currentRadius;

            lineRenderer.SetPosition(i, position);
        }

        lineRenderer.widthMultiplier = Mathf.Lerp(0f, startWidth, 1f - currentRadius / maxRadius);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, maxRadius);
    }
}
