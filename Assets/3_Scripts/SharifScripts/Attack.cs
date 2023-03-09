using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] private Transform attackPos;
    [SerializeField] private float attackRadius;
    [SerializeField] private SoundWave sound;
    public float applyForce;

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Return))
        {
            CheckForEnemies();
            sound.StartCoroutine(sound.Wave());
        }
    }
    void Attck()
    {
        if (Input.GetKey(KeyCode.Return))
        {
            CheckForEnemies();
            
        }
    }

    void CheckForEnemies()
    {
        Collider[] colliders = Physics.OverlapSphere(attackPos.position, attackRadius);

        foreach(Collider enemy in colliders)
        {
            if (enemy.CompareTag("Enemy"))
            {
                Debug.Log(enemy.name);

                Rigidbody rb = enemy.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    Vector3 forceDirection = (enemy.transform.position - attackPos.position).normalized;
                    rb.AddForce(forceDirection * applyForce, ForceMode.Impulse);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPos.position, attackRadius);
    }
}
