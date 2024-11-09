using UnityEngine;

public class HintItem : MonoBehaviour, ICollectible
{
    [SerializeField] private string hintText; // Текст подсказки
    [SerializeField] private GameObject hintUIPrefab; // Префаб UI элемента подсказки

    public string HintText => hintText;

    public void Collect(CollectController player)
    {
        player.AddHintToInventory(this);
        gameObject.SetActive(false); // Отключение объекта после подбора
    }
}
