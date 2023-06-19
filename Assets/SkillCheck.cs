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
    public float decayRate = 5f;
    [SerializeField] private int numOfNodes = 0;
    [SerializeField] private int successPress = -1;
    [SerializeField] private Slider chargeSlider;
    [SerializeField] private Image target;
    [SerializeField] private RectTransform targetTransform;
    public int[] values = { -40, 40};
    public int[] targetPos;
    private float currentChargeLevel = 0f;
    private bool isMaxCharge;
    public bool keyIsDown;
    public int previousValue = 0;

    private void ButtonPressed()
    {
        int randValue = targetPos[Random.Range(0, targetPos.Length)];

        while (randValue == previousValue)
        {
            randValue = targetPos[Random.Range(0, targetPos.Length)];
        }

        target.gameObject.SetActive(true);
        targetTransform.LeanSetLocalPosX(randValue);
        previousValue = randValue;
        Debug.Log("random value: " + randValue);
    }
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

        if (!isMaxCharge)
        {
            currentChargeLevel += decayRate * Time.deltaTime;
            currentChargeLevel = Mathf.Clamp(currentChargeLevel, 0f, maxChargeLevel);
            chargeSlider.value = currentChargeLevel;
        }
        else
        {
            currentChargeLevel -= decayRate * Time.deltaTime;
            currentChargeLevel = Mathf.Clamp(currentChargeLevel, 0f, maxChargeLevel);
            chargeSlider.value = currentChargeLevel;
        }

        if(Input.GetMouseButtonUp(0))
        {
            ButtonPressed();
        }
    }
}
