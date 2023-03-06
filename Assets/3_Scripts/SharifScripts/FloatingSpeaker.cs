using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingSpeaker : MonoBehaviour
{
    public GameObject player;
    public float moveSpeed = 5.0f;
    public FloatingEffect floatingEffect;
    private Vector3 initialPlayerPosition;
    private Vector3 finalPosition;

    //[SerializeField]private Transform bulletPos;

    void Start()
    {
        initialPlayerPosition = player.transform.position;
        finalPosition = transform.position;
    }

    void Update()
    {

        // Set target position for movement
        float newY = floatingEffect.transform.position.y;
        finalPosition = new Vector3(player.transform.position.x, newY, player.transform.position.z);

        // Move towards target using Lerp function
        float journeyLength = Vector3.Distance(initialPlayerPosition, finalPosition);
        float journeyTime = journeyLength / moveSpeed;
        float fracJourney = Mathf.Clamp01(Time.deltaTime / journeyTime);
        transform.position = Vector3.Lerp(transform.position, finalPosition, fracJourney);

    }

    public void Attack()
    {
        /*
        if (receiveInput.attack == true)
        {
            Attack();
        }
        GameObject bullet = ObjectPooling.instance.GetPooledObject();

        if (bullet != null)
        {
            bullet.transform.position = bulletPos.position;
            bullet.SetActive(true);
            receiveInput.attack = false;
        }
        */
    }
}
