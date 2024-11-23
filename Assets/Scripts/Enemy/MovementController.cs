using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WaypointData
{
    public Transform point;
    public bool enableParticles;
    public bool openDoor;
    public float waitDuration;
    public string waypointSoundKey;
    public bool loopWaypointSound;
}
[System.Serializable]
public class HunterModeSettings
{
    public float minTimeUntilHunterMode = 40f;
    public float maxTimeUntilHunterMode = 60f;
    public float hunterModeDuration = 20f;
    public float hunterSpeed = 6f;
    public float teleportSearchThresholdY = 3f;
}
// Класс для управления передвижением
public class MovementController : MonoBehaviour, IMoveable
{
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Move(Vector2 direction, float speed)
    {
        rb.velocity = direction.normalized * speed;
    }

    public void Stop()
    {
        rb.velocity = Vector2.zero;
    }
}
