using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] private Transform attackPos;
    [SerializeField] private float attackRadius;
    [SerializeField] private SoundWave sound;
    [SerializeField] private float cooldown = 1f;
    private float nextAttackTime = 0f;
    public float applyForce;


    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Return) && Time.time >= nextAttackTime)
        {
            AttackAction();
            nextAttackTime= Time.time + cooldown;
        }
    }
    void AttackAction()
    {

        CheckForEnemies();
        sound.StartCoroutine(sound.Wave());
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
