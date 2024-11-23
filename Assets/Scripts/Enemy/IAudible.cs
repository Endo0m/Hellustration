public interface IAudible
{
    void Initialize(string[] soundTypes);
    void PlaySound(string soundKey, bool isLooping = false);
    void StopSound(string soundKey);
    void PlayWaypointSound(string soundKey, bool isLooping);
    void StopWaypointSound();
}