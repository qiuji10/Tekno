using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform
    : MonoBehaviour
{
    [Header("Tick to Enable Platform Types")]
    public bool isMoveable = false;
    public bool isDropable = false;
    public bool isFlipable = false;

    [Header("Moving Platform settings")]
    public Transform[] points;
    private int currentPoint = 0;
    public float speed = 1.0f;
    
    [Header("Shake and Drop Settings")]
    public bool PlayerOnPlatform = false;
    public float shakeDuration = 1.5f;
    public float shakeTimer = 0f; 
   
    [Header("Flip Settings")]
    public float flipInterval = 5f; // The time between flips
    public float flipDuration = 1f;
    public float timeSinceLastFlip;
    private bool flipping = false;

    private GameObject parental;
    public Transform player { get; set; }

    private void Awake()
    {
        parental = transform.GetChild(0).gameObject;
    }

    void Update()
    {
        MovePlatform();
        DropPlatform();
        FlipPlatform();
    }
  

    public void MovePlatform()
    {
        if(isMoveable || isMoveable && isDropable)
        {           
         transform.position = Vector3.MoveTowards(transform.position, points[currentPoint].position, speed * Time.deltaTime);

            if (transform.position == points[currentPoint].position)
            {
                currentPoint++;

                if (currentPoint >= points.Length)
                {
                    currentPoint = 0;
                }   
            }
        }
       
    }

    public void DropPlatform()
    {
        Debug.Log(PlayerOnPlatform);
        if (isDropable)
        {
            if (PlayerOnPlatform)
            {
                shakeTimer += Time.deltaTime;
             

                transform.position = transform.position + Random.insideUnitSphere * 0.2f;

                if (shakeTimer >= shakeDuration)
                {
                    transform.position += new Vector3(0, -3, 0);
                    parental.SetActive(false);
                    player.parent = null;
                }
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
        player.parent = null;
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


