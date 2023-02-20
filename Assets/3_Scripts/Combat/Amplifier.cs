using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amplifier : MonoBehaviour, IDamagaeble
{
    [SerializeField] private int health;
    [SerializeField] private List<EnemyBase> enemiesInControl;

    public bool IsAlive => health > 0;

    public void Damage(int damage)
    {
        if (health > 0)
        {
            health -= damage;
        }
        else
        {
            health = 0;

            if (enemiesInControl != null)
            {
                foreach (EnemyBase enemy in enemiesInControl)
                {
                    enemy.FreeEnemy();
                }
            }
            else
            {
                Debug.Log("no enemies");
            }
        }
    }
}
