using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TeleportAbility : MonoBehaviour
{
    [Header("Teleport Data")]
    [SerializeField] private InputActionReference teleportAction;
    [SerializeField] private MotherNode motherNode;
    [SerializeField] private float teleportRange = 5f;
    [SerializeField] private Transform electricVFX;
    [SerializeField] private SensorDetection tpSensor;
    [SerializeField] private Rigidbody rb;

    [Header("Charging values")]
    [SerializeField] private InputActionReference chargingAction;
    public float maxChargeLevel = 100f;
    public float initialChargeRate = 10f;
    public float maxChargeRate = 50f;
    public float chargeIncreaseAmount = 5f;
    public float rapidPressThreshold = 0.1f;
    public float decayRate = 5f;
    [SerializeField]private Slider chargeSlider;

    private float currentChargeLevel = 0f;
    private float chargeRate = 0f;
    private float timeSinceLastPress = 0f;
    private bool isMaxCharge;
    public bool keyIsDown;

    private void OnEnable()
    {
        teleportAction.action.performed += Teleport;
        chargingAction.action.performed += Charge;
        chargeSlider.gameObject.SetActive(true);
        LeanTween.reset();
    }

    private void OnDisable()
    {
        teleportAction.action.performed -= Teleport;
        chargingAction.action.performed -= Charge;
        chargeSlider.gameObject.SetActive(false);
    }

    private void Charge(InputAction.CallbackContext context)
    {

        if(StanceManager.curTrack.genre != Genre.Electronic)
        {
            return;
        }

        keyIsDown = true;
    }

    private void Update()
    {
        if (keyIsDown)
        {
            keyIsDown = false;
            timeSinceLastPress = 0f;
            currentChargeLevel += chargeIncreaseAmount;
            currentChargeLevel = Mathf.Clamp(currentChargeLevel, 0f, maxChargeLevel);
            chargeSlider.value = currentChargeLevel;
            isMaxCharge = (currentChargeLevel == maxChargeLevel);
        }

        if (keyIsDown && !isMaxCharge)
        {
            keyIsDown = false;
            timeSinceLastPress += Time.deltaTime;

            if (timeSinceLastPress >= rapidPressThreshold)
            {
                chargeRate = Mathf.Lerp(initialChargeRate, maxChargeRate, currentChargeLevel / maxChargeLevel);

                currentChargeLevel += chargeRate * Time.deltaTime;
                currentChargeLevel = Mathf.Clamp(currentChargeLevel, 0f, maxChargeLevel);
                chargeSlider.value = currentChargeLevel;
                isMaxCharge = (currentChargeLevel == maxChargeLevel);
            }
        }
        else
        {
            if (!isMaxCharge)
            {
                currentChargeLevel -= decayRate * Time.deltaTime;
                currentChargeLevel = Mathf.Clamp(currentChargeLevel, 0f, maxChargeLevel);
                chargeSlider.value = currentChargeLevel;
            }
        }

    }
    private void Teleport(InputAction.CallbackContext context)
    {
        motherNode = tpSensor.GetNearestObject<MotherNode>();

        if (isMaxCharge && motherNode != null)
        {
            StartCoroutine(TeleportToNodes());
        }
       
    }

    public void TeleportToNextNode(Transform targetTeleportPoint)
    {
        Vector3 direction = (targetTeleportPoint.position - transform.position).normalized;
        Vector3 centerPoint = Vector3.Lerp(transform.position, targetTeleportPoint.position, 0.5f);
        Transform vfx = Instantiate(electricVFX, targetTeleportPoint.position, Quaternion.LookRotation(direction));
        LeanTween.move(gameObject, targetTeleportPoint.position, TempoManager.GetTimeToBeatCount(1f)).setOnComplete(() => Destroy(vfx.gameObject, 0.35f));
        
    }

    public IEnumerator TeleportToNodes()
    {
        foreach (Transform teleportPoint in motherNode.teleportPoints)
        {
            rb.isKinematic = true;
            
            if (teleportPoint != null )
            {
                
                TeleportToNextNode(teleportPoint);
                yield return new WaitForSeconds(0.3f); // wait for the player to finish teleporting before teleporting again
                rb.isKinematic = false;
                currentChargeLevel = 0;
                isMaxCharge = false;
                
            }

            
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, teleportRange);
    }
}
