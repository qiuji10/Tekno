using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatus : MonoBehaviour 
{
    [SerializeField] private int health = 50;
    private int totalHealth;
    public static int life = 5;

    public int Health { get { return health; } }
    private bool isGlitchy;
    //private CheckpointManager checkpointManager;
    private MaterialModifier matModifier;

    private Animator _anim;

    private SpectrumUI spectrum;

    private void Awake()
    {
        totalHealth = health;

        //checkpointManager = FindObjectOfType<CheckpointManager>();
        matModifier = GetComponentInChildren<MaterialModifier>();
        spectrum = FindObjectOfType<SpectrumUI>();
    }

    private void OnEnable()
    {
        StanceManager.OnStanceChangeStart += StanceManager_OnStanceChangeStart;
    }

    private void OnDisable()
    {
        StanceManager.OnStanceChangeStart -= StanceManager_OnStanceChangeStart;
    }

    private void StanceManager_OnStanceChangeStart(Track track)
    {
        switch (track.genre)
        {
            case Genre.House:
                SetHealthColor(Color.yellow);
                break;
            case Genre.Techno:
                SetHealthColor(Color.cyan);
                break;
            case Genre.Electronic:
                SetHealthColor(Color.green);
                break;
        }

        void SetHealthColor(Color color)
        {
            for (int i = 0; i < spectrum.images.Count; i++)
            {
                if (spectrum.images[i].color != Color.red)
                {
                    spectrum.images[i].color = color;
                }
            }
        }
    }


    public void Damage(int damage)
    {
        if (health > damage)
        {
            health -= damage;

            float ratio = ((float)health / totalHealth) * (float)spectrum.images.Count;

            for (int i = 0; i < spectrum.images.Count; i++)
            {
                if (i > ratio)
                {
                    spectrum.images[i].color = Color.red;
                }
            }

            if (!isGlitchy)
            {
                StartCoroutine(GlitchyDamageEffect());
            }
        }
        else
        {
            health = 0;

            for (int i = 0; i < spectrum.images.Count; i++)
            {
                spectrum.images[i].color = Color.red;
            }

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