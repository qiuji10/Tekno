using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlatformDrop: MonoBehaviour,IPlatform
{
    [Header("Shake and Drop Settings")]
    //public bool playerOnPlatform = false;
    public float shakeDuration = 5f;
    public float shakeTimer = 0;
    public float dropDistance = 10;
    private float originalY;
    private bool isDropping;

    public GameObject parental;
    private Rigidbody rb;
    private Collider platformCollider;
    public Transform player { get; set; }
    public bool PlayerOnPlatform {get; set;}

    private void Awake()
    {
        TryGetComponent(out rb);
        platformCollider = GetComponent<Collider>();
        parental = transform.GetChild(0).gameObject;
        originalY = transform.position.y;
    }

    void Update()
    {
        DropPlatform();
    }


    public void DropPlatform()
    {
        if (PlayerOnPlatform)
        {
            shakeTimer += Time.deltaTime;

            // modify the y shake to 0 to prevent player cant jump
            Vector3 randomOffset = Random.insideUnitSphere * 0.2f;
            randomOffset.y = 0;
            transform.position = transform.position + new Vector3(randomOffset.x, 0, randomOffset.z);

            if (shakeTimer >= shakeDuration)
            {
                isDropping = true;
                transform.position += new Vector3(0, -1, 0);
                platformCollider.enabled = false;
                PlayerOnPlatform = false;
                parental.SetActive(false);
                if (player != null)
                {
                    player.parent = null;
                }
            }
        }

        if (isDropping && transform.position.y < originalY - 20)
        {
            if (shakeTimer != 0) shakeTimer = 0;
            isDropping = false;
        }
        else if (!isDropping && transform.position.y == originalY)
        {
            isDropping = true;
        }

        if (transform.position.y >= originalY)
        {
            parental.SetActive(true);
            isDropping = false;
            platformCollider.enabled = true;
            transform.position = new Vector3(transform.position.x, originalY, transform.position.z);
            shakeTimer = 0;
        }
    }
}


