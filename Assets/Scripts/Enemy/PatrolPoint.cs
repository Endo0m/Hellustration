using UnityEngine;

public class PatrolPoint : MonoBehaviour
{
    [SerializeField] private float animationDuration = 2f; // Длительность анимации в секундах

    public float AnimationDuration => animationDuration;
}
