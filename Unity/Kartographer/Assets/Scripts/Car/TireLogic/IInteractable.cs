using UnityEngine;


/// <summary>
/// Used for any items that are interactable so raycast can hit and get
/// IInteractable and call this function to do whatever is overriden
/// </summary>
public interface IInteractable
{
    void Interact(PlayerInteractor player);
    void Drop(PlayerInteractor player);
}
