using System.Collections;
using UnityEngine;

public class ZombieEnemy : EnemyBase
{
    [Header("Zombie Settings")]
    [SerializeField] private float moanInterval = 10f; // Интервал между изданиями звуков

    private float moanTimer;

    protected override void Start()
    {
        base.Start(); // Вызов базового метода Start
        moanTimer = moanInterval; // Установка начального значения таймера
    }

    protected override void Update()
    {
        base.Update(); // Вызов базового метода Update

        // Специфичное поведение зомби - издание звуков
        moanTimer -= Time.deltaTime;
        if (moanTimer <= 0f)
        {
            Moan();
            moanTimer = moanInterval;
        }
    }

    protected override void HandleDetection()
    {
        base.HandleDetection(); // Вызов базовой логики обнаружения

        // Дополнительное поведение зомби при обнаружении игрока
        if (isChasing)
        {
            // Возможно, вы хотите добавить специфичное поведение, когда зомби преследует игрока
            Debug.Log("Зомби преследует игрока!");
        }
    }

    protected override IEnumerator PlayAnimationAtPoint(Transform targetPoint)
    {
        // Переопределение анимации при достижении точки (например, зомби может рычать или бить)
        isPlayingAnimation = true;

        // Установка уникальной логики для зомби
        Debug.Log("Зомби выполняет анимацию на точке.");

        yield return new WaitForSeconds(stopDuration); // Задержка, имитирующая анимацию

        isPlayingAnimation = false;
    }

    private void Moan()
    {
        // Логика издания звуков (например, проигрывание звука рычания)
        Debug.Log("Зомби рычит!");
    }
}
