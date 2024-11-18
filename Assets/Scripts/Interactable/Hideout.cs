using UnityEngine;

public class Hideout : MonoBehaviour, IInteractable
{
    [SerializeField] private bool faceRightWhenHidden = true; // Если true - лицо вправо, иначе - влево

    public void Interact(GameObject interactor)
    {
        // Проверяем, является ли объект взаимодействия PlayerController
        PlayerController player = interactor.GetComponent<PlayerController>();
        if (player != null)
        {
            player.HideInObject(transform, faceRightWhenHidden);
        }
    }
}
