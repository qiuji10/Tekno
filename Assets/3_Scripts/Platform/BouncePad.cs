using System.Collections;
using UnityEngine;

public class BouncePad : MonoBehaviour
{
    private PlayerController playerController;
    public ParticleSystem smokeVFX;

    [Header("Bounce Strength")]
    [SerializeField] private float baseBounceForce = 23;
    [SerializeField] private float houseBounceForce = 40f;
    [SerializeField] private float technoBounceForce = 50f;
    [SerializeField] private float electroBounceForce = 100f;
    [SerializeField] private float jumpPadBoostDuration = 0.5f;

    private float vfxSimulationSpeed = 1f;
    private bool isPlayerOnPad = false;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
    }

    private void Update()
    {
        ChangeVFXSpeed();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerOnPad = true;
            Track currentTrack = StanceManager.curTrack;

            switch (currentTrack.genre)
            {
                case Genre.House:
                    playerController.defaultJumpForce = houseBounceForce;
                    break;
                case Genre.Techno:
                    playerController.defaultJumpForce = technoBounceForce;
                    break;
                case Genre.Electronic:
                    playerController.defaultJumpForce = electroBounceForce;
                    break;
                default:
                    playerController.defaultJumpForce = baseBounceForce;
                    break;
            }

            Vector3 bounceDirection = transform.up;
            Vector3 bounceForce = bounceDirection * playerController.defaultJumpForce;
            other.GetComponent<Rigidbody>().AddForce(bounceForce, ForceMode.Impulse);

            StartCoroutine(RemoveJumpPadEffect(other.gameObject));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerOnPad = false;
            playerController.defaultJumpForce = baseBounceForce;
        }
    }

    private void ChangeVFXSpeed()
    {
        Track currentTrack = StanceManager.curTrack;

        // Change VFX simulation speed based on the current track genre
        switch (currentTrack.genre)
        {
            case Genre.House:
                vfxSimulationSpeed = 1f;
                break;
            case Genre.Techno:
                vfxSimulationSpeed = 2f;
                break;
            case Genre.Electronic:
                vfxSimulationSpeed = 3f;
                break;
            default:
                vfxSimulationSpeed = 1f; // Default speed
                break;
        }

        var mainModule = smokeVFX.main;
        mainModule.simulationSpeed = vfxSimulationSpeed;
    }

    private IEnumerator RemoveJumpPadEffect(GameObject player)
    {
        yield return new WaitForSeconds(jumpPadBoostDuration);

        if (isPlayerOnPad)
            playerController.defaultJumpForce = baseBounceForce;
    }
}
