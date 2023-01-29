using UnityEngine;

public class PlayerStatus : MonoBehaviour, IDamagaeble 
{
    private int health = 1000;
    public static int life = 5;

    public int Health { get { return health; } }
    public bool IsAlive => health > 0;
    private CheckpointManager checkpointManager;

    private void Awake()
    {
        checkpointManager = FindObjectOfType<CheckpointManager>();
    }

    public void Damage(int damage)
    {
        if (health > damage)
        {
            health -= damage;
        }
        else
        {
            health = 0;
            
            if (life > 0)
            {
                life -= 1;
                checkpointManager.SetPlayerToSpawnPoint(transform);
            }
            else
            {
                // lose game
                checkpointManager.SetPlayerToSpawnPoint(transform);
            }
        }
    }

    private void OnApplicationQuit()
    {
        life = 5;
    }
}