using UnityEngine;

public class PlayerFixInteraction : MonoBehaviour
{
    public KeyCode repairKey = KeyCode.Tab;
    public SkillCheckManager skillUI;

    InteractableFixable fixable;

    void Update()
    {
        if (fixable == null) return;

        if (Input.GetKeyDown(repairKey))
        {
            if (fixable.isFixed) return;
            if (fixable.IsOnCooldown) return;

            skillUI.StartSkillChecks(
                fixable.requiredChecks,
                OnSkillSequenceFinished
            );
        }
    }

    void OnTriggerEnter(Collider other)
    {
        InteractableFixable f = other.GetComponent<InteractableFixable>();
        if (f != null)
            fixable = f;
    }

    void OnTriggerExit(Collider other)
    {
        InteractableFixable f = other.GetComponent<InteractableFixable>();
        if (f == fixable)
            fixable = null;
    }

    void OnSkillSequenceFinished(bool success)
    {
        if (success)
            fixable.OnCompleteSuccess();
        else
            fixable.OnFail();
    }
}
