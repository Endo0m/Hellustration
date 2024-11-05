using UnityEngine;

public class Hideout : MonoBehaviour, IInteractable
{
    public void Interact(PlayerController player)
    {
        player.HideInObject(transform);
    }
}
