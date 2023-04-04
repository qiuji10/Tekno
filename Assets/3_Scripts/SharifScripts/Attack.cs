using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Attack : MonoBehaviour
{
    [SerializeField] private Transform attackPos;
    [SerializeField] private float attackRadius;
    [SerializeField] private SoundWave sound;
    [SerializeField] private float cooldown = 1f;
    private bool isCooldownFinish;

    [SerializeField] private InputActionReference attackAction;

    public bool showDebug;

    private void Awake()
    {
        isCooldownFinish = true;
    }

    private void OnEnable()
    {
        attackAction.action.performed += AttackAction;
    }

    private void OnDisable()
    {
        attackAction.action.performed -= AttackAction;
    }

    private void AttackAction(InputAction.CallbackContext context)
    {
        if (!isCooldownFinish) return;

        isCooldownFinish = false;
        CheckForEnemies();
        StartCoroutine(Cooldown());
        sound.StartCoroutine(sound.Wave());
    }

    private void CheckForEnemies()
    {
        Collider[] colliders = Physics.OverlapSphere(attackPos.position, attackRadius);

        foreach(Collider enemy in colliders)
        {
            if (enemy.CompareTag("Enemy") && enemy.TryGetComponent(out IKnockable knockable))
            {
                Vector3 forceDirection = (enemy.transform.position - attackPos.position).normalized;
                knockable.Knock(forceDirection, 100);
                Debug.Log(enemy.name);
            }
            
        }
    }

    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldown);
        isCooldownFinish = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (showDebug)
            Gizmos.DrawWireSphere(attackPos.position, attackRadius);
    }
}
