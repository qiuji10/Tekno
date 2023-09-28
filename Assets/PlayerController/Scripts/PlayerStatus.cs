using NaughtyAttributes;
using NodeCanvas.BehaviourTrees;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStatus : MonoBehaviour 
{
    [SerializeField] private int health = 50;
    public int Health { get { return health; } }

    [SerializeField] private Sprite blueHead;
    [SerializeField] private Sprite yellowHead;
    [SerializeField] private Sprite greenHead;

    [Header("Death")]
    [SerializeField] private GameObject deathCanvas;
    [SerializeField] private CameraSetter deathCamSetter;

    public bool isGlitchy, isDead;
    private int totalHealth;
    private CheckpointManager checkpointManager;
    private MaterialModifier matModifier;
    private SpectrumUI spectrum;
    private Animator _anim;

    PlayerController pc;
    HookAbility hk;
    TeleportAbility tp;
    Attack at;

    private void Awake()
    {
        totalHealth = health;

        _anim = GetComponentInChildren<Animator>();
        checkpointManager = FindObjectOfType<CheckpointManager>();
        matModifier = GetComponentInChildren<MaterialModifier>();
        spectrum = FindObjectOfType<SpectrumUI>();

        pc = GetComponent<PlayerController>();
        hk = GetComponent<HookAbility>();
        tp = GetComponent<TeleportAbility>();
        at = GetComponent<Attack>();
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
                spectrum.teknoImg.sprite = yellowHead;
                break;
            case Genre.Techno:
                SetHealthColor(Color.cyan);
                spectrum.teknoImg.sprite = blueHead;
                break;
            case Genre.Electronic:
                spectrum.teknoImg.sprite = greenHead;
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

    [Button]
    public void TestDamage()
    {
        Damage(5, false);
    }


    public void Damage(int damage, bool needRespawn)
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

            if (needRespawn)
            {
                if (checkpointManager == null)
                    checkpointManager = FindObjectOfType<CheckpointManager>();

                checkpointManager.SetPlayerToSpawnPoint(transform);
            }
        }
        else
        {
            health = 0;

            for (int i = 0; i < spectrum.images.Count; i++)
            {
                spectrum.images[i].color = Color.red;
            }


            if (!isDead)
            {
                isDead = true;

                GetComponent<Rigidbody>().velocity = Vector3.zero;

                StanceManager manager = FindObjectOfType<StanceManager>();

                if (manager != null)
                {
                    manager.gameObject.SetActive(false);
                }

                PauseMenu menu = FindObjectOfType<PauseMenu>();

                if (menu != null)
                {
                    menu.gameObject.SetActive(false);
                }

                pc.enabled = false;
                hk.enabled = false;
                tp.enabled = false;
                at.enabled = false;

                StartCoroutine(DeathCutscene());
            }

            
        }
    }

    private IEnumerator DeathCutscene()
    {
        deathCamSetter.LoadCam();
        _anim.SetTrigger("IsDeath");
        yield return new WaitForSeconds(6f);
        deathCanvas.SetActive(true);
    }

    public void BackLobby()
    {
        StartCoroutine(BackToLobby());
    }

    private IEnumerator BackToLobby()
    {
        BehaviourTreeOwner[] enemies = FindObjectsOfType<BehaviourTreeOwner>();

        foreach (BehaviourTreeOwner item in enemies)
        {
            item.gameObject.SetActive(false);
            yield return null;
        }

        MaterialModifier modifier = GetComponentInChildren<MaterialModifier>();
        modifier.ResetMaterial();

        FadeCanvas.Instance.FadeOut();
        yield return new WaitForSeconds(1f);

        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene("Base Scene (Elevator)");
    }

    IEnumerator GlitchyDamageEffect()
    {
        isGlitchy = true;
        matModifier.GlitchyEffectOn();
        yield return null;
        yield return new WaitForSeconds(1f);
        matModifier.GlitchyEffectOff();
        yield return null;
        isGlitchy = false;
    }
}