using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using Random = UnityEngine.Random;

public class TeleportAbility : MonoBehaviour
{
    [Header("Teleport Data")]
    [SerializeField] private InputActionReference teleportAction;
    [SerializeField] private InputActionReference skillCheckAction;
    [SerializeField] private MotherNode motherNode;
    [SerializeField] private float teleportRange = 5f;
    [SerializeField] private Transform electricVFX;
    [SerializeField] private SensorDetection tpSensor;
    [SerializeField] private Rigidbody rb;

    [Header("Charging values")]
    [SerializeField] private InputActionReference chargingAction;
    [SerializeField] private int numOfNodes = 0;
    [SerializeField] private int successPress = -1;
    [SerializeField]private GameObject chargeSlider;

    [Header("Skill Check")]
    public Image[] targetGameObject;
    public Image handleImage;
    public Image barNormal;
    public Image teleportBar;
    public Image barSuccess;
    public Image barFail;
    public int previousValue = 0;
    private int spawnNum = 5;
    public int counter;
    public int randIndex;
    private bool success;
    private bool canTeleport = false;
    public float delayTime;
    private bool pauseCounter = false;

    private void OnEnable()
    {
        TempoManager.OnBeat += TempoManager_OnBeat;
        skillCheckAction.action.performed += SkillCheck;
        counter = -1;
        randIndex = Random.Range(1, spawnNum);
        successPress = -1;
        motherNode = null;
        LeanTween.reset();
    }

    private void OnDestroy()
    {
        TempoManager.OnBeat -= TempoManager_OnBeat;
        motherNode = null;
    }

    private void OnDisable()
    {
        skillCheckAction.action.performed -= SkillCheck;
        TempoManager.OnBeat -= TempoManager_OnBeat;
        motherNode = null;
        successPress = -1;
    }

    private void TempoManager_OnBeat()
    {
        if (StanceManager.curTrack.genre != Genre.Electronic)
        {
            return;
        }

        if (!pauseCounter)
        {
            counter++;
        }
        
        //Debug.Log(counter);

        if (counter == 0 || counter == 5)
        {
            handleImage.gameObject.SetActive(false);
            handleImage.rectTransform.position = targetGameObject[counter].transform.position;
        }
        else
        {
            handleImage.gameObject.SetActive(true);
            handleImage.rectTransform.position = targetGameObject[counter].transform.position;
        }


        for (int i = 0; i < targetGameObject.Length; i++)
        {

            if (i != randIndex)
            {
                targetGameObject[i].color = Color.white;
            }
        }

        targetGameObject[randIndex].color = Color.green;

        if (counter == 5)
        {
            counter = -1;
        }
    }

    private void SkillCheck(InputAction.CallbackContext context)
    {
        motherNode = tpSensor.GetNearestObject<MotherNode>();


        if (StanceManager.curTrack.genre != Genre.Electronic)
        {
            return;
        }

        if (motherNode != null)
        {
            pauseCounter = false;
            chargeSlider.gameObject.SetActive(true);
            numOfNodes = motherNode.teleportPoints.Count;

            if (counter == randIndex)
            {
                success = true;
                successPress++;
                pauseCounter = true;
                Debug.Log("Success");
                StartCoroutine(ChangeSprite());
                StartCoroutine(ChangeTarget());
            }
            else if(successPress == -1 && counter != randIndex)
            {
                success = true;
                successPress++;
            }
            else
            {
                
                success = false;
                pauseCounter = true;
                Debug.Log("Fail");
                StartCoroutine(ChangeSprite());
                StartCoroutine(ChangeTarget());

            }

            if (numOfNodes == 0)
            {
                canTeleport = false;
                return;
            }
            else if (successPress == numOfNodes)
            {
                canTeleport = true;
                Debug.Log("Here");
                Teleport();

            }

        }
        else
        {
            pauseCounter = true;
            successPress = -1;
            chargeSlider.gameObject.SetActive(false);
        }

        previousValue = randIndex;
    }


    private void Update()
    {


    }

    private void Teleport()
    {
        
        motherNode = tpSensor.GetNearestObject<MotherNode>();

        rb.isKinematic = false;

        if (motherNode != null && canTeleport)
        {
            StartCoroutine(TeleportToNodes());
        }

        successPress = -1;
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
                
            }
            chargeSlider.gameObject.SetActive(false);

        }
    }

    IEnumerator ChangeTarget()
    {
        yield return new WaitForSeconds(delayTime);

        randIndex = Random.Range(1, spawnNum);
        while (randIndex == previousValue)
        {
            randIndex = Random.Range(1, spawnNum);
        }
    }

    IEnumerator ChangeSprite()
    {
        if (success)
        {
            teleportBar.sprite = barSuccess.sprite;
            teleportBar.color = Color.green;
        }
        else
        {
            teleportBar.sprite = barFail.sprite;
            teleportBar.color = Color.red;
            
        }
        yield return new WaitForSeconds(delayTime);
        counter = -1;
        pauseCounter = false;
        teleportBar.sprite = barNormal.sprite;
        teleportBar.color = Color.green;
        handleImage.gameObject.SetActive(true);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, teleportRange);
    }
}
