public interface ICombinable
{
    string CombinedResultPrefabName { get; }
    bool CanCombineWith(string targetItemName);
}
