using UnityEngine;

public class PlayerInputBlocker : MonoBehaviour
{
    public bool inputBlocked = false;

    public bool IsBlocked()
    {
        return inputBlocked;
    }
}
