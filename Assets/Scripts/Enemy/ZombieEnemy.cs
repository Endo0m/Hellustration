using UnityEngine;

public class ZombieEnemy : EnemyBase
{
    protected override void Attack()
    {
        // Реализация атаки зомби
        Debug.Log("Зомби атакует игрока");
        // Здесь можно добавить логику нанесения урона игроку
    }
}

