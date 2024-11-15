using UnityEngine;

public class ItemLifespan : MonoBehaviour
{
    public float lifespan = -1f; // ����� ����� �������� (-1 ��������, ��� ������� �� �������� �� �������)
    private bool isDestroyed = false; // ����, ��������������� ��������� �����������
    public int requiredWaypoint = 1; // �����, ������� ���� ������ ������ ��� ����������� ��������

    private void Start()
    {
        // ���� lifespan ������ 0, ��������� ������ �� �����������
        if (lifespan > 0)
        {
            Invoke(nameof(DestroyItem), lifespan);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isDestroyed && other.CompareTag("Enemy"))
        {
            EnemyBase enemyBase = other.GetComponent<EnemyBase>();
            if (enemyBase != null)
            {
                Debug.Log($"Enemy reached waypoint {enemyBase.CurrentWaypoint}. Required waypoint: {requiredWaypoint}");
                if (enemyBase.CurrentWaypoint >= requiredWaypoint)
                {
                    DestroyItem();
                }
            }
        }
    }


    private void DestroyItem()
    {
        if (!isDestroyed)
        {
            Destroy(gameObject);
            isDestroyed = true;
        }
    }
}
