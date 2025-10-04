using System;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState I { get; private set; }

    public bool P1HasController { get; private set; }
    public bool P2HasController { get; private set; }

    public int P1AssignedSpot = -1;
    public int P2AssignedSpot = -1;

    public event Action<int> OnFound;       // who found (1/2)
    public event Action<int> OnBothFound;   // last finder (1/2)

    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
    }

    public void ResetState()
    {
        P1HasController = P2HasController = false;
        P1AssignedSpot = P2AssignedSpot = -1;
    }

    public void AssignSpots(int p1Index, int p2Index)
    {
        P1AssignedSpot = p1Index;
        P2AssignedSpot = p2Index;
    }

    public bool IsCorrectSpot(int playerIndex, int spotIndex)
    {
        if (playerIndex == 1) return spotIndex == P1AssignedSpot;
        if (playerIndex == 2) return spotIndex == P2AssignedSpot;
        return false;
    }

    public void SetFound(int playerIndex)
    {
        bool changed = false;
        if (playerIndex == 1 && !P1HasController) { P1HasController = true; changed = true; }
        if (playerIndex == 2 && !P2HasController) { P2HasController = true; changed = true; }
        if (!changed) return;

        OnFound?.Invoke(playerIndex);
        if (P1HasController && P2HasController) OnBothFound?.Invoke(playerIndex);
    }
}
