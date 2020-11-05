public interface IConstruction
{
    bool IsAbleToStartConstruction { get; }
    bool IsUnderConstruction { get; }

    void OnConstructionStarted();
    void OnConstructionFinished();
    void OnConstructionCanceled();
}