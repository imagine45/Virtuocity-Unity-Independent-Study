using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class ControlsScript : MonoBehaviour
{
    [SerializeField] private int bindingIndex;
    [SerializeField] private InputActionReference bind = null;
    [SerializeField] private GameObject manager = null;
    private PlayerInput playerInput;
    [SerializeField] private TMP_Text bindingDisplayNameText =null;
    [SerializeField] private GameObject startRebindObject = null;
    [SerializeField] private GameObject waitingForInputObject = null;
    private Color color;

    void Start()
    {
        playerInput = manager.GetComponent<PlayerInput>();
    }

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    public void startRebinding()
    {
        startRebindObject.SetActive(false);
        waitingForInputObject.SetActive(true);

        playerInput.SwitchCurrentActionMap("UI");
        
        rebindingOperation = bind.action.PerformInteractiveRebinding()
            .WithCancelingThrough("<Keyboad>/escape")
            .OnCancel(operation => RebindComplete())
            .OnComplete(operation => RebindComplete())
            .Start();
    }

    public void sizeOnHover()
    {
        this.GetComponent<Image>().color = new Color(0.1f, 0.8f, 0.97f);
        this.color.a = 200f / 255f;
    }

    public void sizeOnExit()
    {
        this.GetComponent<Image>().color = color;
        this.color.a = 200f / 255f;
    }

    private void RebindComplete()
    {
        bindingDisplayNameText.text = InputControlPath.ToHumanReadableString(bind.action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);

        rebindingOperation.Dispose();
        startRebindObject.SetActive(true);
        waitingForInputObject.SetActive(false);
        playerInput.SwitchCurrentActionMap("Player");
    }
}
