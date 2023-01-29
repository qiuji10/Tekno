using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Platform: MonoBehaviour
{
    [Header("Tick to Enable Platform Types")]
    public bool isMoveable = false;
    public bool isDropable = false;
    public bool isFlipable = false;

    [Header("Moving Platform settings")]
    public Transform[] points;
    private int currentPoint = 0, prevPoint;
    public float timeToPoint = 7f;
    
    [Header("Shake and Drop Settings")]
    public bool PlayerOnPlatform = false;
    public float shakeDuration = 1.5f;
    public float shakeTimer = 0f;
    public float dropDistance = 10;
    private float originalY;
    private bool isDropping;
   
    [Header("Flip Settings")]
    public float flipInterval = 5f; // The time between flips
    public float flipDuration = 1f;
    public float timeSinceLastFlip;
    private bool flipping = false;

    private GameObject parental;
    private Rigidbody rb;
    private Collider platformCollider;
    public Transform player { get; set; }

    private void Awake()
    {
        TryGetComponent(out rb);
        platformCollider = GetComponent<Collider>();
        parental = transform.GetChild(0).gameObject;
        originalY = transform.position.y;

        prevPoint = points.Length - 1;
    }

    void Update()
    {
       
        DropPlatform();
        FlipPlatform();
    }

    private void FixedUpdate()
    {
        MovePlatform();
    }

    float timer = 0;

    public void MovePlatform()
    {
        if(isMoveable || isMoveable && isDropable)
        {


            //float movePercent = timer / timeToPoint;
            //movePercent = Mathf.SmoothStep(0, 1, movePercent);
            //transform.position = Vector3.Lerp(points[prevPoint].position, points[currentPoint].position, movePercent);
            Vector3 dir = (points[currentPoint].position - points[prevPoint].position).normalized;
            transform.position = transform.position + dir * dropDistance * Time.deltaTime;
            //  timer += Time.fixedDeltaTime;
            if (Vector3.Distance(transform.position, points[currentPoint].position) < 0.1f)
            {
                prevPoint = currentPoint;
                currentPoint++;
                timer = 0;

                if (currentPoint >= points.Length)
                {
                    currentPoint = 0;
                }   
            }
        }
       
    }

    public void DropPlatform()
    {
        if (isDropable)
        {
            if (PlayerOnPlatform)
            {
                shakeTimer += Time.deltaTime;

                // modify the y shake to 0 to prevent player cant jump
                Vector3 randomOffset = Random.insideUnitSphere * 0.2f;
                randomOffset.y = 0;
                transform.position = transform.position + new Vector3(randomOffset.x, 0, randomOffset.z);
            }

            if (isDropping && transform.position.y < originalY - 20)
            {
                if (shakeTimer != 0) shakeTimer = 0;
                isDropping = false;
            }
            else if (!PlayerOnPlatform && !isDropping)
            {
                transform.position += new Vector3(0, 0.2f, 0);
            }

            if (transform.position.y >= originalY)
            {
                parental.SetActive(true);
                isDropping = false;
                platformCollider.enabled = true;
                transform.position = new Vector3(transform.position.x, originalY, transform.position.z);
                
            }

            if (shakeTimer >= shakeDuration)
            {
                isDropping = true;
                transform.position += new Vector3(0, -1, 0);
                platformCollider.enabled = false;
                PlayerOnPlatform = false;
                parental.SetActive(false);
                player.parent = null;
            }
        }


    }

    public void FlipPlatform()
    {
        if (isFlipable)
        {
            if (!flipping)
            {
                timeSinceLastFlip += Time.deltaTime;
                if (timeSinceLastFlip >= flipInterval)
                {
                    StartCoroutine(Flip());
                    
                    timeSinceLastFlip = 0;
                }
            }
        }
    }


    IEnumerator Flip()
    {
        parental.SetActive(false);
        if (player != null && player.parent != null) player.SetParent(null);
        flipping = true;
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(180f, 0f, 0f);
        float time = 0f;

        while (time <= flipDuration)
        {
            time += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, time / flipDuration);
            
            yield return null;
        }
        flipping = false;
        parental.SetActive(true);   
    }
}


