using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCore : MonoBehaviour, IDamagable
{
    [SerializeField] private int health;
    [SerializeField] private int maxHealth = 100;

    public bool IsAlive => health > 0;

    void Start()
    {
        // might change due to scene transition and wanna remain health
        health = maxHealth;
    }

    public void Damage(int damage)
    {
        health -= damage;

        if (health <= 0f)
        {
            health = 0;
            Die();
        }
    }

    public void Die()
    {

    }
}
