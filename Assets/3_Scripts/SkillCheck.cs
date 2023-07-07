using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class SkillCheck : MonoBehaviour
{
    [Header("Charging values")]
    //[SerializeField] private InputActionReference chargingAction;
    public float maxChargeLevel = 50f;
    public float chargeRate = 5f;
    [SerializeField] private int numOfNodes = 0;
    [SerializeField] private int successPress = -1;
    [SerializeField] private Slider chargeSlider;
    [SerializeField] private Image target;
    [SerializeField] private RectTransform targetTransform;
    public Image[] targetGameObject;
    private float currentChargeLevel = 0f;
    private bool isMaxCharge;
    public int previousValue = 0;
    private int spawnNum = 4;
    public float spawnDelay;
    public float disableDelay;
    private bool startCharge = false;
    private int startIndex;
    private int currentIndex;

    
    private void Start()
    {
        chargeSlider.maxValue = maxChargeLevel;
        target.gameObject.SetActive(false);

    }
    private void Update()
    {
        if (currentChargeLevel == maxChargeLevel)
        {
            isMaxCharge = true;
            //target.gameObject.SetActive(false);
        }

        if (currentChargeLevel <= 0)
        {
            isMaxCharge = false;
            //target.gameObject.SetActive(false);
        }

        if (!isMaxCharge && startCharge)
        {
            currentChargeLevel += chargeRate * Time.deltaTime;
            currentChargeLevel = Mathf.Clamp(currentChargeLevel, 0f, maxChargeLevel);
            chargeSlider.value = currentChargeLevel;
        }
        else
        {
            //currentChargeLevel -= decayRate * Time.deltaTime;
            currentChargeLevel = 0;
            currentChargeLevel = Mathf.Clamp(currentChargeLevel, 0f, maxChargeLevel);
            chargeSlider.value = currentChargeLevel;
        }

        if (Input.GetMouseButtonUp(0))
        {
            ButtonPressed();
        }
        
    }

    private void Charge()
    {

        if (!isMaxCharge)
        {
            currentChargeLevel += chargeRate * Time.deltaTime;
            currentChargeLevel = Mathf.Clamp(currentChargeLevel, 0f, maxChargeLevel);
            chargeSlider.value = currentChargeLevel;
        }
        else
        {
            //currentChargeLevel -= decayRate * Time.deltaTime;
            currentChargeLevel = 0;
            currentChargeLevel = Mathf.Clamp(currentChargeLevel, 0f, maxChargeLevel);
            chargeSlider.value = currentChargeLevel;
        }
    }
    IEnumerator SpawnPoints()
    {
        yield return new WaitForSeconds(spawnDelay);
        int randValue = Random.Range(0, spawnNum); // Randomly select the starting index

        while (randValue == previousValue)
        {
            randValue = Random.Range(0, spawnNum);
        }

        for (int i = 0; i < spawnNum; i++)
        {

            if (i == randValue)
            {
                targetGameObject[i].color = Color.green;
            }
            else
            {
                targetGameObject[i].color = Color.white;
            }

            targetGameObject[i].gameObject.SetActive(true);
        }


        previousValue = randValue;
    }

    private void ButtonPressed()
    {
        startCharge = true;
        StartCoroutine(SpawnPoints());
        
    }
}
