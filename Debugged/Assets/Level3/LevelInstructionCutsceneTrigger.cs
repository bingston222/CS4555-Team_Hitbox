using UnityEngine;

public class LevelInstructionCutsceneTrigger : MonoBehaviour
{
    public GameObject dialogueCanvas; // assign your DialogueCanvas here
    bool triggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;
            dialogueCanvas.SetActive(true); // starts the DialogueController cutscene
        }
    }
}

