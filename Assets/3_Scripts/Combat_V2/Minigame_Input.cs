using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class Minigame
{
    public enum SpeakerStatus { Ready, On, Off, Success }

    [System.Serializable]
    public class InputReference
    {
        public InputActionReference circle;
        public InputActionReference cross;
        public InputActionReference square;
        public InputActionReference triangle;
    }

    [System.Serializable]
    public class InputData
    {
        public KeyInput key;
        public float beatTime;
        public bool pressed;
    }

    [System.Serializable]
    public class Input
    {
        public bool startTrace;
        public KeyInput key = KeyInput.None;
        public KeyInput prevKey = KeyInput.None;
        public Vector2 hitPos;
        public Vector2 prevHitPos;
        public RectTransform area;

        private int curBeat;
        private int totalInput, successInput;

        private InputReference inputRef;

        [SerializeField] bool keyPressed;
        [SerializeField] KeyInput pressedKey;

        [SerializeField] float gracePeriod = 0.6f;
        [SerializeField] float earlyPeriod = 0.15f;

        [SerializeField] List<InputData> inputs = new();

        public event Action OnComboSuccess;
        public event Action OnBeatSuccess;
        public event Action<string> OnBeatFailure;

        public bool done => successInput == totalInput;
        private float secPerBeat => 60f / TempoManager.staticBPM;

        public Input(InputReference inputRef)
        {
            this.inputRef = inputRef;

            inputRef.circle.action.performed += OnPressedKey;
            inputRef.cross.action.performed += OnPressedKey;
            inputRef.square.action.performed += OnPressedKey;
            inputRef.triangle.action.performed += OnPressedKey;
        }

        ~Input()
        {
            inputRef.circle.action.performed -= OnPressedKey;
            inputRef.cross.action.performed -= OnPressedKey;
            inputRef.square.action.performed -= OnPressedKey;
            inputRef.triangle.action.performed -= OnPressedKey;
        }

        public void Init(List<BeatData> data, RectTransform areaCoverage)
        {
            successInput = 0;
            totalInput = data.Where(b => b.key != KeyInput.None).Count();
            hitPos = prevHitPos = data[0].position;
            area = areaCoverage;

            float curBeat = TempoManager._lastBeatTime;
            float secondsPerBeat = 60f / TempoManager.staticBPM;

            inputs.Clear();

            for (int i = 0; i < data.Count; i++)
            {
                var inputData = new InputData()
                {
                    key = data[i].key,
                    beatTime = curBeat + secondsPerBeat * i,
                };

                inputs.Add(inputData);
            }
        }

        public void Process(int syncCurBeat)
        {
            if (syncCurBeat >= inputs.Count && !done)
                FalseInput("<color=red>Last Beat Time Out</color>");

            if (curBeat < syncCurBeat)
                curBeat = Mathf.Clamp(syncCurBeat, 0, inputs.Count - 1);
                //curBeat = Mathf.Clamp(syncCurBeat, -1, inputs.Count);

            if (!startTrace) return;

            if (keyPressed)
            {
                KeyInput curKey = inputs[curBeat].key;
                float earlyTime = inputs[curBeat].beatTime + secPerBeat + secPerBeat - earlyPeriod;
                //float earlyTime = inputs[curBeat].beatTime + secPerBeat - earlyPeriod;
                bool noKey = inputs[curBeat].key == KeyInput.None;
                bool early = Time.time < earlyTime;

                //Debug.Log($"b: {syncCurBeat}, earlyTime: {earlyTime}, curTime: {Time.time}");

                keyPressed = false;


                InputData prevInput = null;

                if (curBeat > 0)
                {
                    prevInput = inputs[curBeat - 1];
                }

                if (prevInput != null && prevInput.key != KeyInput.None && !prevInput.pressed)
                {
                    prevInput.pressed = true;
                    early = false;

                    if (pressedKey == prevInput.key)
                    {
                        successInput++;
                        OnBeatSuccess?.Invoke();

                        if (successInput == totalInput)
                        {
                            OnComboSuccess?.Invoke();
                            startTrace = false;
                        }
                    }
                    else
                    {
                        FalseInput($"<color=yellow>Wrong Key {prevInput.key} - {pressedKey}</color>");
                        return;
                    }

                    return;
                }
                else
                {
                    inputs[curBeat].pressed = true;

                    if (curKey == KeyInput.None)
                    {
                        FalseInput($"<color=yellow>Wrong Key {curKey} - {pressedKey}</color>");
                        return;
                    }

                }

                if (noKey) return;

                if (early)
                {
                    FalseInput($"<color=yellow>Early {Time.time} - {earlyTime}</color>");
                    return;
                }

                if (pressedKey == curKey)
                {
                    successInput++;
                    OnBeatSuccess?.Invoke();

                    if (successInput == totalInput)
                    {
                        OnComboSuccess?.Invoke();
                        startTrace = false;
                    }
                }
                else
                {
                    FalseInput($"<color=red>Wrong Key Pressed {curKey} - {pressedKey}</color>");
                }
            }
            else
            {
                if (curBeat <= 0 || curBeat > inputs.Count + 1)
                    return;

                int beat = Mathf.Clamp(curBeat, 0, inputs.Count - 1);

                float lateTime = inputs[beat].beatTime /*+ secPerBeat  */+ gracePeriod;
                bool timeout = Time.time > lateTime;
                bool haveKey = inputs[curBeat - 1].key != KeyInput.None;
                bool inputPressed = inputs[curBeat - 1].pressed;

                if (haveKey && timeout && !inputPressed)
                    FalseInput($"<color=red>Timeout {curBeat} {Time.time} - {lateTime}</color>");
            }
        }

        private void OnPressedKey(InputAction.CallbackContext ctx)
        {
            if (!startTrace) return;

            KeyInput input = (KeyInput)Enum.Parse(typeof(KeyInput), ctx.action.name, ignoreCase: true);

            pressedKey = input;
            keyPressed = true;
        }

        private void FalseInput(string msg)
        {
            startTrace = false;
            OnBeatFailure?.Invoke(msg);
            Debug.Log(msg);
        }
    }
}
