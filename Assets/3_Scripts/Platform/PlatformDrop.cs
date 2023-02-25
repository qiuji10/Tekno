    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlatformDrop: MonoBehaviour
{
    [Header("Shake and Drop Settings")]
    public bool PlayerOnPlatform = false;
    public float shakeDuration = 1.5f;
    public float shakeTimer = 0f;
    public float dropDistance = 10;
    private float originalY;
    private bool isDropping;

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
            if (shakeTimer != 0) shakeTimer = 0;

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


