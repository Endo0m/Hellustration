public interface ICollectible
{
    void Collect(CollectController player);
    string CollectSoundKey { get; }
}
