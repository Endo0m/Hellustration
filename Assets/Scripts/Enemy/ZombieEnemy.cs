using UnityEngine;

public class ZombieEnemy : EnemyBase
{
    protected override void HandleDetection()
    {
        // Реализуем логику обнаружения для SimpleEnemy
        Debug.Log("SimpleEnemy выполняет свою логику обнаружения!");
    }

  
}

