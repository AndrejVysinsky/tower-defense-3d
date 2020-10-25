using UnityEngine.Events;
using UnityEngine.EventSystems;

public interface IInteractionChanged : IEventSystemHandler
{
    void OnInteractionAdded(UnityAction action);
    void OnInteractionRemoved(string interactionName, UnityAction action);
    void OnInteractionHidden();
}
