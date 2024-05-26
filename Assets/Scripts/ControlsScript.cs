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

    private void RebindComplete()
    {
        bindingDisplayNameText.text = InputControlPath.ToHumanReadableString(bind.action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);

        rebindingOperation.Dispose();
        startRebindObject.SetActive(true);
        waitingForInputObject.SetActive(false);
        playerInput.SwitchCurrentActionMap("Player");
    }
}
