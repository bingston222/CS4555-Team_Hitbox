using UnityEngine;
using UnityEngine.UI;

public class UIControllerIcons : MonoBehaviour
{
    public Image p1Icon, p2Icon;

    void Start()
    {
        if (p1Icon) p1Icon.enabled = GameState.I.P1HasController;
        if (p2Icon) p2Icon.enabled = GameState.I.P2HasController;

        // one event for either player
        GameState.I.OnFound += who =>
        {
            if (who == 1 && p1Icon) p1Icon.enabled = true;
            if (who == 2 && p2Icon) p2Icon.enabled = true;
        };
    }
}

