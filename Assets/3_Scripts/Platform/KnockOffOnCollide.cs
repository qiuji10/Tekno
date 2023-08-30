using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockOffOnCollide : MonoBehaviour
{
    [SerializeField] private float knockForce = 100f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player") && collision.transform.TryGetComponent(out Rigidbody rb))
        {
            Vector3 direction = (collision.GetContact(0).point - transform.position).normalized;
            rb.AddForce(direction * knockForce, ForceMode.Impulse);
        }
    }
}
