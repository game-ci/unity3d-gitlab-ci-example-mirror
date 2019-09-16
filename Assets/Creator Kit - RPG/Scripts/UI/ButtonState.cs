using UnityEngine.UI;

public class ButtonState : Button
{
    private bool pressUp;
    private bool pressDown;
    private bool press;

    public bool PressDown => pressDown;
    public bool PressUp   => pressUp;
    public bool Press     => press;

    private void Update()
    {
        bool newPress = IsPressed();
        pressUp       = !newPress && press;
        pressDown     = newPress && !press;
        press         = newPress;
    }
}
