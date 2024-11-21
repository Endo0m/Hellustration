using UnityEngine;

public class WobbleObject : MonoBehaviour
{
    [SerializeField] private Animator animator; // �������� ��� ���������� ������������
    [SerializeField] private GameObject itemToDrop; // �������, ������� �������
    [SerializeField] private Transform dropPoint; // �����, � ������� ������� �������
    [SerializeField] private Vector2 forceDirection = new Vector2(1f, 1f); // ����������� ������
    [SerializeField] private float forceStrength = 5f; // ���� ������
    [SerializeField] private string openSoundKey; // ���� ��� ����� ��������
    private AudioSource audioSource;
    private bool hasDroppedItem = false; // ����, ��������������� ��������� ���������

    public void Interact(GameObject interactor)
    {
        if (hasDroppedItem) return; // ������������� ��������� ��������������
        SoundManager.Instance.PlaySound(openSoundKey, audioSource);

        // ������ �������� �����������
        if (animator != null)
        {
            animator.SetTrigger("Wobble");
        }

        // ��������� ��������
        DropItem();
    }

    private void DropItem()
    {
        if (itemToDrop != null && !hasDroppedItem)
        {
            // ������� ��������� ��������
            GameObject droppedItem = Instantiate(itemToDrop, dropPoint != null ? dropPoint.position : transform.position, Quaternion.identity);

            // ���������� ���� � Rigidbody2D, ���� �� ����
            Rigidbody2D rb2D = droppedItem.GetComponent<Rigidbody2D>();
            if (rb2D != null)
            {
                rb2D.AddForce(forceDirection.normalized * forceStrength, ForceMode2D.Impulse);
            }
            else
            {
                // ���������� ���� � Rigidbody (��� 3D)
                Rigidbody rb = droppedItem.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(new Vector3(forceDirection.x, forceDirection.y, 0f).normalized * forceStrength, ForceMode.Impulse);
                }
            }

            hasDroppedItem = true; // ������������� ���� ����� ������� ���������
        }
    }

    // ���������� ����� ��� ����������� ������
    private void OnDrawGizmosSelected()
    {
        if (dropPoint == null) return;

        // ������������� ���� ��� �����
        Gizmos.color = Color.red;

        // ��������� ����� - ����� ������� ��� ������� �������
        Vector3 startPoint = dropPoint.position;
        Vector3 endPoint = startPoint + new Vector3(forceDirection.x, forceDirection.y, 0f).normalized * forceStrength;

        // ������ �������
        Gizmos.DrawLine(startPoint, endPoint);
        Gizmos.DrawSphere(endPoint, 0.1f); // ��������� ������ �� ����� �������
    }
}
