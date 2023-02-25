using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speed = 10;

    [SerializeField]private Rigidbody rb;
    private Vector3 enemyTransform;
    private GameObject enemy;

    private void Start()
    {
        StartCoroutine(DisableAfterDelay(1f));
        //GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");

        if(enemy != null)
        {
            enemyTransform = enemy.transform.position;
        }
    }

    private void FixedUpdate()
    {
        enemy = GameObject.FindGameObjectWithTag("Enemy");
        if (enemy != null)
        {
            enemyTransform = enemy.transform.position;
        }

        if (enemyTransform != null)
        {
            Vector3 direction = (enemyTransform - transform.position).normalized;
            rb.velocity = direction * speed;
        }
        else
        {
            rb.velocity = Vector3.forward * speed;
        }

        StartCoroutine(DisableAfterDelay(2f));

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            gameObject.SetActive(false);
        }
    }

    private IEnumerator DisableAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        DisableObj();
    }

    public void DisableObj()
    {
        gameObject.SetActive(false);
    }
}
