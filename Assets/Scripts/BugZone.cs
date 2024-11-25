using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugZone : MonoBehaviour
{
    [Header("Zone Settings")]
    [SerializeField] private float width = 10f;
    [SerializeField] private float height = 10f;

    public bool IsPointInZone(Vector2 point)
    {
        Vector2 localPoint = transform.InverseTransformPoint(point);
        return Mathf.Abs(localPoint.x) <= width / 2 && Mathf.Abs(localPoint.y) <= height / 2;
    }

    public Vector2 GetRandomPointInZone()
    {
        float randomX = Random.Range(-width / 2, width / 2);
        float randomY = Random.Range(-height / 2, height / 2);
        return transform.TransformPoint(new Vector2(randomX, randomY));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 1f, 0f, 0.3f); // Полупрозрачный жёлтый
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawCube(Vector3.zero, new Vector3(width, height, 0.1f));

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(width, height, 0.1f));
    }

    public Vector2 ClampPointToZone(Vector2 point)
    {
        Vector2 localPoint = transform.InverseTransformPoint(point);
        localPoint.x = Mathf.Clamp(localPoint.x, -width / 2, width / 2);
        localPoint.y = Mathf.Clamp(localPoint.y, -height / 2, height / 2);
        return transform.TransformPoint(localPoint);
    }
}