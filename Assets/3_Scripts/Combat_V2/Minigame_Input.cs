using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering.LookDev;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class Minigame
{
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

        public event Action OnSuccess;
        public event Action<string> OnFailure;

        private bool entered;
        private int totalInput, successInput;

        private InputReference inputRef;
        private MonoBehaviour mono;
        private Coroutine coroutine;

        private bool InArea => RectTransformUtility.RectangleContainsScreenPoint(area, hitPos);
        private bool InPrevArea => RectTransformUtility.RectangleContainsScreenPoint(area, prevHitPos);

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

        public void Init(MonoBehaviour mono, List<BeatData> data, RectTransform areaCoverage)
        {
            successInput = 0;
            totalInput = data.Where(b => b.key != KeyInput.None).Count();
            hitPos = prevHitPos = data[0].position;
            area = areaCoverage;
            this.mono = mono;

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

            //EnableUpdateCheck();
        }

        [SerializeField] bool keyPressed;
        [SerializeField] KeyInput pressedKey;

        [SerializeField] float gracePeriod = 0.35f;
        [SerializeField] float earlyPeriod = 0.235f;

        [SerializeField] List<InputData> inputs = new();

        float secPerBeat => 60f / TempoManager.staticBPM;

        int curBeat;

        public void Process(int syncCurBeat)
        {
            if (curBeat < syncCurBeat)
                curBeat = syncCurBeat;

            if (!startTrace) return;

            if (keyPressed)
            {
                KeyInput curKey = inputs[curBeat].key;
                float earlyTime = inputs[curBeat].beatTime + secPerBeat + secPerBeat - earlyPeriod;

                Debug.Log($"<color=yellow>TEST {curBeat} {Time.time} - {earlyTime}</color>");

                bool haveKey = inputs[curBeat].key != KeyInput.None;
                bool early = Time.time < earlyTime;


                keyPressed = false;
                inputs[curBeat].pressed = true;

                if (early)
                {
                    FalseInput($"<color=yellow>Early {Time.time} - {earlyTime}</color>");
                }

                if (haveKey)
                {
                    if (pressedKey == curKey)
                    {
                        successInput++;

                        if (successInput == totalInput)
                        {
                            startTrace = false;
                            OnSuccess?.Invoke();
                        }
                    }
                    else
                    {
                        FalseInput($"<color=red>Wrong Key Pressed {pressedKey} - {curKey}</color>");
                    }
                }
                //else
                //{
                //    FalseInput($"<color=red>Wrong Key Pressed</color>");
                //}
            }
            else
            {
                if (curBeat <= 0 || curBeat > inputs.Count + 1)
                    return;

                float lateTime = inputs[curBeat - 1].beatTime + gracePeriod;
                bool timeout = Time.time > lateTime;
                bool haveKey = inputs[curBeat - 1].key != KeyInput.None;
                bool inputPressed = inputs[curBeat - 1].pressed;

                if (haveKey && timeout && !inputPressed)
                    FalseInput($"<color=red>Timeout {curBeat - 1} {Time.time} - {lateTime}</color>");
            }
        }

        private void OnPressedKey(InputAction.CallbackContext ctx)
        {
            if (!startTrace) return;

            KeyInput input = (KeyInput)Enum.Parse(typeof(KeyInput), ctx.action.name, ignoreCase: true);

            //if (!entered)
            //{
            //    FalseInput("<color=red>Too Early</color>");
            //}

            pressedKey = input;
            keyPressed = true;
            entered = false;


            //if (input != key)
            //{
            //    FalseInput("<color=red>Wrong Key Pressed</color>");
            //    return;
            //}

            //if (InArea)
            //{
            //    successInput++;

            //    if (successInput == totalInput)
            //        OnSuccess?.Invoke();
            //}
        }

        private void FalseInput(string msg)
        {
            startTrace = false;
            OnFailure?.Invoke(msg);
            Debug.Log(msg);
        }
    }
}
