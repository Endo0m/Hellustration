using UnityEngine;

public class Hideout : MonoBehaviour, IInteractable
{
    [SerializeField] private bool faceRightWhenHidden = true; // ���� true - ���� ������, ����� - �����

    public void Interact(GameObject interactor)
    {
        // ���������, �������� �� ������ �������������� PlayerController
        PlayerController player = interactor.GetComponent<PlayerController>();
        if (player != null)
        {
            player.HideInObject(transform, faceRightWhenHidden);
        }
    }
}
