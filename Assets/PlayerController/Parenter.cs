using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parenter : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Transform player = collision.transform;

            float playerHeight = transform.localScale.y / 2;

            if (player.position.y > playerHeight && player.position.y < playerHeight + 0.1f)
            {
                return;
            }

            player.position = new Vector3(player.position.x, transform.position.y + transform.localScale.y / 2, player.position.z);
        }
    }
}
