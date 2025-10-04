using UnityEngine;

public interface IInteractable
{
    string GetPromptText();
    void Interact(Interactor interactor);
    Transform transform { get; } // allows distance checks
}
