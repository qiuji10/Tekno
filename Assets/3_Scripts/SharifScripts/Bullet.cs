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
    [SerializeField] private EnemyDetection enemyDetection;
    [SerializeField] private FloatingSpeaker floatingSpeaker;

    private void Start()
    {
        //StartCoroutine(DisableAfterDelay(1f));
        enemyDetection = GameObject.FindGameObjectWithTag("Detect").GetComponent<EnemyDetection>();
        floatingSpeaker = GameObject.FindGameObjectWithTag("Player").GetComponent<FloatingSpeaker>();

        
    }

    private void FixedUpdate()
    {

        enemy = enemyDetection.closeEnemy;


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
