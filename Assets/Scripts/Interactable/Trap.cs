using UnityEngine;

public class Trap : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyBase enemy = other.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            enemy.TriggerDeath(); // ������ �������� ����������� �����
            Destroy(gameObject); // ����������� ����� �������
        }
    }
}
