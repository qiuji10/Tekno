using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReceiver : MonoBehaviour
{
    public static InputReceiver Instance;

    [Header("Settings")]
    public bool hideCursorOnStart;

    [Header("Reference")]
    public Vector2 move;
    public Vector2 look;
    public bool jump, attack, heavyAttack, square;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        if (hideCursorOnStart)
            CursorOff();
    }

    public void CursorOn()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void CursorOff()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    //public void OnMove(InputValue value)
    //{
    //    MoveInput(value.Get<Vector2>());
    //}

    //public void OnLook(InputValue value)
    //{
    //    LookInput(value.Get<Vector2>());
    //}

    //public void OnCross(InputValue value)
    //{
    //    JumpInput(value.isPressed);
    //}

    //public void OnCircle(InputValue value)
    //{
    //    SquareInput(value.isPressed);
    //}

    //public void OnSquare(InputValue value)
    //{
    //    AttackInput(value.isPressed);
    //}

    //public void OnTriangle(InputValue value)
    //{
    //    HeavyAttack(value.isPressed);
    //}

    //public void MoveInput(Vector2 newMoveDirection)
    //{
    //    move = newMoveDirection;
    //}
    
    //public void LookInput(Vector2 newLookDirection)
    //{
    //    look = newLookDirection;
    //}

    //public void JumpInput(bool newJump)
    //{
    //    jump = newJump;
    //}

    //public void AttackInput(bool attack)
    //{
    //    this.attack = attack;
    //}

    //public void HeavyAttack(bool heavyAttack)
    //{
    //    this.heavyAttack = heavyAttack;
    //}

    //public void SquareInput(bool square)
    //{
    //    this.square = square;
    //}

    public bool AnyActionKey()
    {
        return attack || jump || square || heavyAttack;
    }

    public static bool ReceiveInput(KeyInput key)
    {
        switch (key)
        {
            case KeyInput.Circle:
                return InputReceiver.Instance.attack;
            case KeyInput.Cross:
                return InputReceiver.Instance.jump;
            case KeyInput.Square:
                return InputReceiver.Instance.square;
            case KeyInput.Triangle:
                return InputReceiver.Instance.heavyAttack;
        }

        return false;
    }

    public static void ToggleOffInput(KeyInput key)
    {
        switch (key)
        {
            case KeyInput.Circle:
                InputReceiver.Instance.attack = false;
                break;
            case KeyInput.Cross:
                InputReceiver.Instance.jump = false;
                break;
            case KeyInput.Square:
                InputReceiver.Instance.square = false;
                break;
            case KeyInput.Triangle:
                InputReceiver.Instance.heavyAttack = false;
                break;
        }
    }

    public static bool IsWrongInput(KeyInput key)
    {
        switch (key)
        {
            case KeyInput.Circle:
                return !InputReceiver.Instance.attack;
            case KeyInput.Cross:
                return !InputReceiver.Instance.jump;
            case KeyInput.Square:
                return !InputReceiver.Instance.square;
            case KeyInput.Triangle:
                return !InputReceiver.Instance.heavyAttack;
        }
        return false;
    }

    public static void ToggleOffAllInput()
    {
        InputReceiver.Instance.attack = false;
        InputReceiver.Instance.jump = false;
        InputReceiver.Instance.square = false;
        InputReceiver.Instance.heavyAttack = false;
    }
}
