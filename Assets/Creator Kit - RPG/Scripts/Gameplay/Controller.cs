using UnityEngine;

public abstract class Controller : MonoBehaviour
{
    public abstract bool GetInputDown(InputButton input);
    public abstract bool GetInputUp(InputButton input);
    public abstract bool GetInput(InputButton input);
}
