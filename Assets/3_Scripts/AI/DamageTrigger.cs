using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    [SerializeField] private int damage = 5;

    private Collider triggerCollider;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider>();
        triggerCollider.enabled = false;
    }

    public void TriggerCollider(bool trigger)
    {
        triggerCollider.enabled = trigger;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamagable damagable))
        {
            //Debug.Log("<color=blue>damage</color>");
            damagable.Damage(damage);
        }
    }
}
