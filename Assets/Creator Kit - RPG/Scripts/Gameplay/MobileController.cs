using System;
using System.Collections.Generic;
using UnityEngine;

public class MobileController : Controller, ISerializationCallbackReceiver
{
    [SerializeField] private List<InputButtonState> allInputKeys = new List<InputButtonState>();
    private Dictionary<InputButton, ButtonState> inputsToButtonStates = new Dictionary<InputButton, ButtonState>();

    public override bool GetInput(InputButton input)
    {
        var button = GetKeyCodeFromInputButton(input);
        return button ? button.Press : false;
    }

    public override bool GetInputDown(InputButton input)
    {
        var button = GetKeyCodeFromInputButton(input);
        return button ? button.PressDown : false;
    }

    public override bool GetInputUp(InputButton input)
    {
        var button = GetKeyCodeFromInputButton(input);
        return button ? button.PressUp : false;
    }

    private ButtonState GetKeyCodeFromInputButton(InputButton input)
    {
        if (inputsToButtonStates.TryGetValue(input, out ButtonState keyCode)) {
            return keyCode;
        }

        return null;
    }

    public void OnAfterDeserialize()
    {
        inputsToButtonStates.Clear();
        InputButtonState inputButtonState;

        for (int i = allInputKeys.Count - 1; i > -1; i--) {
            inputButtonState = allInputKeys[i];
            inputsToButtonStates.Add(inputButtonState.inputButton, inputButtonState.buttonState);
        }
    }

    public void OnBeforeSerialize()
    {
        InputButton input;

        foreach (var enumObj in Enum.GetValues(typeof(InputButton))) {
            input = (InputButton) enumObj;

            if (allInputKeys.Find(inputButtonState => inputButtonState.inputButton == input) == null) {
                allInputKeys.Add(new InputButtonState() {
                    inputButton = input
                });
            }
        }
    }

    [Serializable]
    public class InputButtonState
    {
        public InputButton inputButton;
        public ButtonState buttonState;
    }
}
