using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    [Header("Scene Transition")]
    public SceneTransition sceneTransition;

    [Header("Skill Checks Required")]
    public InteractableFixable[] requiredFixables;

    [Header("Players")]
    public int totalPlayers = 2;
    private int playersInRoom = 0;

    private bool IsPlayer(Collider other)
    {
        // Accept both tags
        return other.CompareTag("Player1") || other.CompareTag("Player2");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsPlayer(other))
            return;

        playersInRoom++;

        // Only trigger when enough players are inside AND all checks done
        if (playersInRoom >= totalPlayers && AllSkillChecksDone())
        {
            if (sceneTransition != null)
            {
                sceneTransition.BeginTransition();   // <--- use BeginTransition
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsPlayer(other))
            return;

        playersInRoom = Mathf.Max(playersInRoom - 1, 0);
    }

    private bool AllSkillChecksDone()
    {
        if (requiredFixables == null || requiredFixables.Length == 0)
            return false;

        foreach (var fixable in requiredFixables)
        {
            if (fixable == null || !fixable.isFixed)
                return false;
        }

        return true;
    }
}
