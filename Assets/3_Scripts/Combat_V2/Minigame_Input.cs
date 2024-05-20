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
    public class Input
    {
        public bool startTrace;
        public KeyInput key = KeyInput.None;
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
            EnableUpdateCheck();
        }

        public void StartCheck(Vector2 newHitPos, KeyInput newKey)
        {
            //if (prevKey != KeyInput.None && startTrace)
            //{
            //    FalseInput("Timeout");
            //    return;
            //}

            prevHitPos = hitPos;
            hitPos = newHitPos;

            //key = newKey;
            ChangeKey(newKey);
            startTrace = true;
        }

        private void ChangeKey(KeyInput key)
        {
            mono.StartCoroutine(DelayChangeKey());

            IEnumerator DelayChangeKey()
            {
                yield return new WaitForSeconds(0.2f);
                this.key = key;
            }
        }

        public void EnableUpdateCheck()
        {
            if (coroutine != null)
                mono.StopCoroutine(CheckOutOfRange());

            coroutine = mono.StartCoroutine(CheckOutOfRange());
        }

        private IEnumerator CheckOutOfRange()
        {
            while (true)
            {
                if (!startTrace) yield return null;

                if (!entered && InArea && key != KeyInput.None)
                {
                    entered = true;
                }
                else if (entered && !InPrevArea)
                {
                    FalseInput("<color=red>Out of Time</color>");
                }

                yield return null;
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

            entered = false;
            startTrace = false;

            if (input != key)
            {
                FalseInput("<color=red>Wrong Key Pressed</color>");
                return;
            }

            if (InArea)
            {
                successInput++;

                if (successInput == totalInput)
                    OnSuccess?.Invoke();
            }
        }

        private void FalseInput(string msg)
        {
            OnFailure?.Invoke(msg);
        }
    }
}
