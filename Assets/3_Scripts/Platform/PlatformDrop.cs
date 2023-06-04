using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlatformDrop : MonoBehaviour, IPlatform
{
    [Header("Shake and Drop Settings")]
    public float shakeDuration = 5f;
    private float shakeTimer = 0f;
    private float originalY;
    private bool isDropping;

    public GameObject parental;
    private Rigidbody rb;
    private Collider platformCollider;
    public Transform player { get; set; }
    public bool PlayerOnPlatform { get; set; }

    private void Awake()
    {
        TryGetComponent(out rb);
        platformCollider = GetComponent<Collider>();
        parental = transform.GetChild(0).gameObject;
        originalY = transform.position.y;
    }

    private void Update()
    {
        DropPlatform();
    }

    public void DropPlatform()
    {
        if (PlayerOnPlatform)
        {
            shakeTimer += Time.deltaTime;

            // Modify the y shake to 0 to prevent player from jumping
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
        else if (!isDropping)
        {
            if (transform.position.y == originalY)
            {
                shakeTimer = 0;
                isDropping = true;
            }
            else
            {
                transform.position += new Vector3(0, 0.2f, 0);
            }
        }

        if (isDropping && transform.position.y < originalY - 20)
        {
            isDropping = false;
        }

        if (transform.position.y >= originalY)
        {
            parental.SetActive(true);
            isDropping = false;
            platformCollider.enabled = true;
            transform.position = new Vector3(transform.position.x, originalY, transform.position.z);
        }
    }
}