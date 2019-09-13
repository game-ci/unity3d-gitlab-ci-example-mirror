using System;
using System.Collections.Generic;
using UnityEngine;

public class StandaloneController : Controller, ISerializationCallbackReceiver
{
    [SerializeField] private List<InputKey> allInputKeys = new List<InputKey>();

    private Dictionary<InputButton, KeyCode> inputsToKeycodes = new Dictionary<InputButton, KeyCode>();

    public override bool GetInput(InputButton input)
    {
        return Input.GetKey(GetKeyCodeFromInputButton(input));
    }

    public override bool GetInputDown(InputButton input)
    {
        return Input.GetKeyDown(GetKeyCodeFromInputButton(input));
    }

    public override bool GetInputUp(InputButton input)
    {
        return Input.GetKeyUp(GetKeyCodeFromInputButton(input));
    }

    private KeyCode GetKeyCodeFromInputButton(InputButton input)
    {
        if (inputsToKeycodes.TryGetValue(input, out KeyCode keyCode)) {
            return keyCode;
        }

        return KeyCode.None;
    }

    public void OnAfterDeserialize()
    {
        inputsToKeycodes.Clear();
        InputKey inputKey;

        for (int i = allInputKeys.Count - 1; i > -1; i--) {
            inputKey = allInputKeys[i];
            inputsToKeycodes.Add(inputKey.inputButton, inputKey.keyCode);
        }
    }

    public void OnBeforeSerialize()
    {
        InputButton input;

        foreach (var enumObj in Enum.GetValues(typeof(InputButton))) {
            input = (InputButton) enumObj;

            if (allInputKeys.Find(inputKey => inputKey.inputButton == input) == null) {
                allInputKeys.Add(new InputKey() {
                    inputButton = input
                });
            }
        }
    }

    [Serializable]
    public class InputKey
    {
        public InputButton inputButton;
        public KeyCode keyCode;
    }
}