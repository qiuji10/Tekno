using UnityEngine;

public class PlayerStatus : MonoBehaviour, IDamagaeble 
{
    private int health = 1000;
    public static int life = 5;

    public bool IsAlive => health > 0;

    public void Damage(int damage)
    {
        if (health >= damage)
        {
            health -= damage;
        }
        else
        {
            health = 0;
            
            if (life > 0)
            {
                life -= 1;
            }
            else
            {
                // lose game
            }
        }
    }
}