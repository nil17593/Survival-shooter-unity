namespace SaveSystem
{
    /// <summary>
    /// Defines the contract for components that need to serialize their state.
    /// </summary>
    public interface ISaveable
    {
        // Writes component state to the save data object
        void Save(GameSaveData data);

        // Restores component state from the save data object
        void Load(GameSaveData data);
    }
}