using UnityEngine;
using UnityEngine.InputSystem;

public class BeatInputHandler : MonoBehaviour
{
    [SerializeField] private InputActionReference hitAction;
    [SerializeField] private InputActionReference recalibrateAction;

    [SerializeField] private int inputCount = 0;
    [SerializeField] private int testCount = 16;

    [SerializeField] private float avgInputDelay;
    public static float inputLag;

    private void OnEnable()
    {
        hitAction.action.performed += Action_performed;
        recalibrateAction.action.performed += Recalibrate;
    }

    private void OnDisable()
    {
        hitAction.action.performed -= Action_performed;
        recalibrateAction.action.performed -= Recalibrate;
        avgInputDelay = 0;

    }

    private void Action_performed(InputAction.CallbackContext obj)
    {
        if (inputCount == testCount) return;

        float expectedBeatTime = Mathf.Floor((Time.time / (60f / 140f)) + 0.5f) * (60f / 140f);
        float currentInputTime = Time.time;
        float currentDelay = currentInputTime - expectedBeatTime;
        float inputDelay = currentDelay * 1000f; // Convert to milliseconds
        Debug.Log(inputDelay + "Input Delay: " + inputDelay.ToString("F2") + "ms"); // Convert to milliseconds

        inputCount++;

        avgInputDelay += inputDelay;

        if (inputCount == testCount)
        {
            inputLag = (avgInputDelay / testCount);
            Debug.Log($"<color=cyan>Input Delay: {inputLag.ToString("F2")} ms</color>");
        }
    }

    private void Recalibrate(InputAction.CallbackContext obj)
    {
        inputCount = 0;
        avgInputDelay = 0;
    }
}