using UnityEngine;

public class ItemLifespan : MonoBehaviour
{
    public float lifespan = 5f; // ����� ����� �������� � ��������
    private bool isDestroyed = false; // ���� ��� �������������� ���������� �����������

    private void Start()
    {
        // ������ ������� ��� ����������� �������
        Invoke(nameof(DestroyItem), lifespan);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // �������� �� ��������� ����� � ���������� ���������
        if (!isDestroyed && other.CompareTag("Enemy"))
        {
            DestroyItem();
        }
    }

    private void DestroyItem()
    {
        if (!isDestroyed)
        {
            Destroy(gameObject);
            isDestroyed = true; // ��������� ����� ��� �������������� ���������� �����������
        }
    }
}
