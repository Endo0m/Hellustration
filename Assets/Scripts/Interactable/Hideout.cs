using UnityEngine;

public class Hideout : MonoBehaviour, IInteractable
{
    public void Interact(GameObject interactor)
    {
        // ���������, �������� �� ������ �������������� PlayerController
        PlayerController player = interactor.GetComponent<PlayerController>();
        if (player != null)
        {
            player.HideInObject(transform);
        }
    }
}
