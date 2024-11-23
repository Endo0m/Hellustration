using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetector : MonoBehaviour, IPlayerDetector
{
    private LayerMask playerLayer;
    private LayerMask hideLayer;

    public void Initialize(LayerMask playerLayer, LayerMask hideLayer, float frontLength, float backLength)
    {
        this.playerLayer = playerLayer;
        this.hideLayer = hideLayer;
    }

    public bool DetectPlayer(Vector2 position, Vector2 direction, float frontLength, float backLength)
    {
        Transform player = GetDetectedPlayer(position, direction, frontLength, backLength);
        return player != null;
    }

    public Transform GetDetectedPlayer(Vector2 position, Vector2 direction, float frontLength, float backLength)
    {
        Transform detectedPlayer = CheckRaycast(position, direction, frontLength);
        if (detectedPlayer == null)
        {
            detectedPlayer = CheckRaycast(position, -direction, backLength);
        }
        return detectedPlayer;
    }

    private Transform CheckRaycast(Vector2 position, Vector2 direction, float length)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(position, direction, length);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.CompareTag("Player") &&
                hit.collider.gameObject.layer != LayerMask.NameToLayer("Hidden"))
            {
                return hit.collider.transform;
            }
        }
        return null;
    }
}