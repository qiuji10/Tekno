using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerControllerAdjustor : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private TMP_Text moveSpeedText;
    [SerializeField] private TMP_Text airSpeedText;
    [SerializeField] private TMP_Text moveDragText;
    [SerializeField] private TMP_Text jumpForceText;
    [SerializeField] private TMP_Text fallMultiplierText;
    [SerializeField] private TMP_Text lowJumpMultiplierText;
    [SerializeField] private Slider moveSpeedSlider;
    [SerializeField] private Slider airSpeedSlider;
    [SerializeField] private Slider moveDragSlider;
    [SerializeField] private Slider jumpForceSlider;
    [SerializeField] private Slider fallMultiplierSlider;
    [SerializeField] private Slider lowJumpMultiplierSlider;

    [SerializeField] private Transform resetTransform;

    private void Start()
    {
        UpdateSliderValues();
        UpdateTextValues();

        moveSpeedSlider.onValueChanged.AddListener(ModifyMoveSpeed);
        airSpeedSlider.onValueChanged.AddListener(ModifyAirSpeed);
        moveDragSlider.onValueChanged.AddListener(ModifyMoveDrag);
        jumpForceSlider.onValueChanged.AddListener(ModifyJumpForce);
        fallMultiplierSlider.onValueChanged.AddListener(ModifyFallMultiplier);
        lowJumpMultiplierSlider.onValueChanged.AddListener(ModifyLowJumpMultiplier);
    }

    private void UpdateSliderValues()
    {
        moveSpeedSlider.value = playerController.defaultMoveSpeed;
        airSpeedSlider.value = playerController.defaultAirSpeed;
        moveDragSlider.value = playerController.defaultMoveDrag;
        jumpForceSlider.value = playerController.defaultJumpForce;
        fallMultiplierSlider.value = playerController.defaultFallMultiplier;
        lowJumpMultiplierSlider.value = playerController.defaultLowJumpMultiplier;
    }

    private void UpdateTextValues()
    {
        moveSpeedText.text = "Move Speed: " + playerController.defaultMoveSpeed.ToString();
        airSpeedText.text = "Air Speed: " + playerController.defaultAirSpeed.ToString();
        moveDragText.text = "Move Drag: " + playerController.defaultMoveDrag.ToString();
        jumpForceText.text = "Jump Force: " + playerController.defaultJumpForce.ToString();
        fallMultiplierText.text = "Fall Multiplier: " + playerController.defaultFallMultiplier.ToString();
        lowJumpMultiplierText.text = "Low Jump Multiplier: " + playerController.defaultLowJumpMultiplier.ToString();
    }

    private void Awake()
    {
        // Set the initial TMP_Text values with the default values from PlayerController
        moveSpeedText.text = "Move Speed: " + playerController.defaultMoveSpeed.ToString();
        airSpeedText.text = "Air Speed: " + playerController.defaultAirSpeed.ToString();
        moveDragText.text = "Move Drag: " + playerController.defaultMoveDrag.ToString();
        jumpForceText.text = "Jump Force: " + playerController.defaultJumpForce.ToString();
        fallMultiplierText.text = "Fall Multiplier: " + playerController.defaultFallMultiplier.ToString();
        lowJumpMultiplierText.text = "Low Jump Multiplier: " + playerController.defaultLowJumpMultiplier.ToString();
    }

    public void ModifyMoveSpeed(float value)
    {
        playerController.defaultMoveSpeed = value;
        moveSpeedText.text = "Move Speed: " + value.ToString();
    }

    public void ModifyAirSpeed(float value)
    {
        playerController.defaultAirSpeed = value;
        airSpeedText.text = "Air Speed: " + value.ToString();
    }

    public void ModifyMoveDrag(float value)
    {
        playerController.defaultMoveDrag = value;
        moveDragText.text = "Move Drag: " + value.ToString();
    }

    public void ModifyJumpForce(float value)
    {
        playerController.defaultJumpForce = value;
        jumpForceText.text = "Jump Force: " + value.ToString();
    }

    public void ModifyFallMultiplier(float value)
    {
        playerController.defaultFallMultiplier = value;
        fallMultiplierText.text = "Fall Multiplier: " + value.ToString();
    }

    public void ModifyLowJumpMultiplier(float value)
    {
        playerController.defaultLowJumpMultiplier = value;
        lowJumpMultiplierText.text = "Low Jump Multiplier: " + value.ToString();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            playerController.transform.position = resetTransform.position;
        }
    }

}