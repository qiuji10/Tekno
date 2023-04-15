using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargingSystem : MonoBehaviour
{
    public float maxChargeLevel = 100f;
    public float initialChargeRate = 10f;
    public float maxChargeRate = 50f;
    public float chargeIncreaseAmount = 5f;
    public float rapidPressThreshold = 0.1f;
    public float decayRate = 5f;

    private float currentChargeLevel = 0f;
    private float chargeRate = 0f;
    private float timeSinceLastPress = 0f;
    public bool isMaxCharge;

    public Slider chargeSlider;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            timeSinceLastPress = 0f;
            currentChargeLevel += chargeIncreaseAmount;
            currentChargeLevel = Mathf.Clamp(currentChargeLevel, 0f, maxChargeLevel);
            chargeSlider.value = currentChargeLevel;
            isMaxCharge = (currentChargeLevel == maxChargeLevel);
        }

        if (Input.GetKey(KeyCode.X) && !isMaxCharge)
        {
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
}
