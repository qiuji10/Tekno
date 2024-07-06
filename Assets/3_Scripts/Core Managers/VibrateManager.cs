using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VibrateManager : MonoBehaviour
{
    private Gamepad gamepad;
    private Coroutine rumbleRoutine;

    public static VibrateManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    [SerializeField] private float lowFreq = 0.25f;
    [SerializeField] private float highFreq = 0.1f;
    [SerializeField] private float duration = 5f;

    [Button] void SetTime1() => Time.timeScale = 1f;
    [Button] void SetTime0() => Time.timeScale = 0f;
    [Button] void SetMotor() => Rumble(lowFreq, highFreq, duration);

    public void Rumble(float lowFreq, float highFreq, float duration)
    {
        gamepad = Gamepad.current;

        if (gamepad != null)
        {
            if (rumbleRoutine != null)
                StopCoroutine(rumbleRoutine);

            rumbleRoutine = StartCoroutine(RumbleRoutine(lowFreq, highFreq, duration));
        }
    }

    private IEnumerator RumbleRoutine(float lowFreq, float highFreq, float duration)
    {
        float timer = 0;

        gamepad.SetMotorSpeeds(lowFreq, highFreq);

        while (timer < duration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        gamepad.SetMotorSpeeds(0, 0);
    }
}
