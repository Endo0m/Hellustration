using System.Collections;
using UnityEngine;

public class ZombieEnemy : EnemyBase
{
    [Header("Zombie Settings")]
    [SerializeField] private float moanInterval = 10f; // �������� ����� ��������� ������

    private float moanTimer;

    protected override void Start()
    {
        base.Start(); // ����� �������� ������ Start
        moanTimer = moanInterval; // ��������� ���������� �������� �������
    }

    protected override void Update()
    {
        base.Update(); // ����� �������� ������ Update

        // ����������� ��������� ����� - ������� ������
        moanTimer -= Time.deltaTime;
        if (moanTimer <= 0f)
        {
            Moan();
            moanTimer = moanInterval;
        }
    }

    protected override void HandleDetection()
    {
        base.HandleDetection(); // ����� ������� ������ �����������

        // �������������� ��������� ����� ��� ����������� ������
        if (isChasing)
        {
            // ��������, �� ������ �������� ����������� ���������, ����� ����� ���������� ������
            Debug.Log("����� ���������� ������!");
        }
    }

    protected override IEnumerator PlayAnimationAtPoint(Transform targetPoint)
    {
        // ��������������� �������� ��� ���������� ����� (��������, ����� ����� ������ ��� ����)
        isPlayingAnimation = true;

        // ��������� ���������� ������ ��� �����
        Debug.Log("����� ��������� �������� �� �����.");

        yield return new WaitForSeconds(stopDuration); // ��������, ����������� ��������

        isPlayingAnimation = false;
    }

    private void Moan()
    {
        // ������ ������� ������ (��������, ������������ ����� �������)
        Debug.Log("����� �����!");
    }
}
