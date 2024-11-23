using UnityEngine;

public class ItemLifespan : MonoBehaviour
{
    [SerializeField] private float lifespan = -1f;
    [SerializeField] private int requiredWaypoint = 5; // �������� �� ���������
    private bool isDestroyed = false;
    private int spawnCycle = -1;
    private bool isInitialized = false;

    public void Initialize(int spawnRequiredWaypoint)
    {
        // ���������� requiredWaypoint �� ����������, ���� �� �� ��� ���������������
        if (!isInitialized)
        {
            isInitialized = true;
            // ���������� �������� �� ����������, ���� ��� ������ ��� ����������
            requiredWaypoint = Mathf.Max(requiredWaypoint, spawnRequiredWaypoint);
        }

        if (lifespan > 0)
        {
            Invoke(nameof(DestroyItem), lifespan);
        }

        Debug.Log($"Item initialized with requiredWaypoint: {requiredWaypoint}, lifespan: {lifespan}");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isDestroyed && other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                int currentWaypoint = enemy.CurrentWaypoint;
                int waypointCount = enemy.WaypointCount;
                int currentCycle = currentWaypoint / waypointCount;
                int waypointInCycle = currentWaypoint % waypointCount;

                // ���������� ���� ��� ������ ����� � �������
                if (spawnCycle == -1)
                {
                    spawnCycle = currentCycle;
                }

                Debug.Log($"Item check: Cycle {currentCycle}, Waypoint {waypointInCycle}, " +
                    $"Required {requiredWaypoint}, SpawnCycle {spawnCycle}, " +
                    $"Will destroy: {currentCycle == spawnCycle && waypointInCycle >= requiredWaypoint}");

                // ���������, ��� ��� ��� �� ���� � ���������� ������ �����
                if (currentCycle == spawnCycle && waypointInCycle >= requiredWaypoint)
                {
                    Debug.Log($"Destroying item at waypoint {waypointInCycle} (required: {requiredWaypoint})");
                    DestroyItem();
                }
            }
        }
    }

    private void DestroyItem()
    {
        if (!isDestroyed)
        {
            Debug.Log($"Item destroyed! Required waypoint was: {requiredWaypoint}");
            Destroy(gameObject);
            isDestroyed = true;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // �������� � ��������� ��� ����������� � ������������ ���������
        if (requiredWaypoint < 0)
        {
            Debug.LogWarning("Required waypoint should not be negative!");
            requiredWaypoint = 0;
        }
    }
#endif
}