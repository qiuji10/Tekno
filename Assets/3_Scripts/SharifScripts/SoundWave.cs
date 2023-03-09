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

    private void Update()
    {
        if(Input.GetKey(KeyCode.V))
        {
            StartCoroutine(Wave());
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, maxRadius);
    }
}
