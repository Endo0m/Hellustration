using UnityEngine;

public class ItemLifespan : MonoBehaviour
{
    public float lifespan = 5f; // ����� ����� �������� � ��������

    private void Start()
    {
        Destroy(gameObject, lifespan); // ����������� ������� ����� ��������� �����
    }
}
