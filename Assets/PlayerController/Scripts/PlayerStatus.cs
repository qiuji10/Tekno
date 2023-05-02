using System.Collections;
using UnityEngine;

public class PlayerStatus : MonoBehaviour 
{
    private int health = 1000;
    public static int life = 5;

    public int Health { get { return health; } }
    private bool isGlitchy;
    //private CheckpointManager checkpointManager;
    private MaterialModifier matModifier;

    private Animator _anim;

    private void Awake()
    {
        //checkpointManager = FindObjectOfType<CheckpointManager>();
        matModifier = GetComponentInChildren<MaterialModifier>();
    }

    public void Damage(int damage)
    {
        if (health > damage)
        {
            health -= damage;
            
            if (!isGlitchy)
            {
                StartCoroutine(GlitchyDamageEffect());
            }
        }
        else
        {
            health = 0;
            
            if (life > 0)
            {
                life -= 1;
                //checkpointManager.SetPlayerToSpawnPoint(transform);
            }
            else
            {
                // lose game
                //checkpointManager.SetPlayerToSpawnPoint(transform);
            }
        }
    }

    IEnumerator GlitchyDamageEffect()
    {
        isGlitchy = true;
        matModifier.GlitchyEffectOn();
        yield return new WaitForSeconds(1f);
        matModifier.GlitchyEffectOff();
        isGlitchy = false;
    }

    private void OnApplicationQuit()
    {
        life = 5;
    }
}