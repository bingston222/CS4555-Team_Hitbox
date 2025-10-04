using UnityEngine;

public class ControllerPickup : MonoBehaviour, IInteractable
{
    [Range(1,2)] public int ownerPlayerIndex = 1;
    public bool restrictToOwner = true;

    public string GetPromptText()
{
    // Try to find the Interactor of the correct player
    var players = GameObject.FindGameObjectsWithTag("Player");
    foreach (var p in players)
    {
        var id = p.GetComponent<PlayerId>();
        var interactor = p.GetComponent<Interactor>();
        if (id != null && interactor != null && id.playerIndex == ownerPlayerIndex)
        {
            return $"Pick up P{ownerPlayerIndex} Controller ({interactor.useKey})";
        }
    }

    // Fallback if nothing found
    return $"Pick up P{ownerPlayerIndex} Controller";
}


    public void Interact(Interactor interactor)
    {
        var id = interactor.GetComponent<PlayerId>();
        if (id == null) return;

        if (restrictToOwner && id.playerIndex != ownerPlayerIndex)
            return; // wrong player - ignore

        GameState.I.SetFound(ownerPlayerIndex);
        Destroy(gameObject);
    }
}
