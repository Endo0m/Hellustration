using UnityEngine;

public interface IMoveable
{
    void Move(Vector2 direction, float speed);
    void Stop();
}