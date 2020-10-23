
public interface IPriorityQueue<TItem, TPriority>
{
    /// Inserts and item with a priority
    void Insert(TItem item, TPriority priority);

    /// Returns the element with the highest priority
    TItem Peek();

    /// Deletes and returns the element with the highest priority
    TItem Pop();
}
