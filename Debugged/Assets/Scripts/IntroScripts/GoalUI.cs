using UnityEngine;
using TMPro;

public class GoalUI : MonoBehaviour
{
    public TMP_Text goalText;

    void Start()
    {
        goalText.text = "Goal: Find the two missing controllers";
        UpdateCount();

        // when anyone finds one, update the (x/2) text
        GameState.I.OnFound += _ => UpdateCount();

        // when both are found, change the goal line
        GameState.I.OnBothFound += _ =>
        {
            goalText.text = "Goal: Connect both controllers to the PC";
        };
    }

    void UpdateCount()
    {
        int c = (GameState.I.P1HasController ? 1 : 0) + (GameState.I.P2HasController ? 1 : 0);
        goalText.text = $"Goal: Find the two missing controllers ({c}/2)";
    }
}
