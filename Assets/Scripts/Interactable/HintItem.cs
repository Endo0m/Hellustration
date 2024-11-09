using UnityEngine;

public class HintItem : MonoBehaviour, ICollectible
{
    [SerializeField] private string hintText; // ����� ���������
    [SerializeField] private GameObject hintUIPrefab; // ������ UI �������� ���������

    public string HintText => hintText;

    public void Collect(CollectController player)
    {
        player.AddHintToInventory(this);
        gameObject.SetActive(false); // ���������� ������� ����� �������
    }
}
