using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InteractionTrigger : MonoBehaviour, IPointerDownHandler
{
    private UnityEvent _unityAction;

    public void SetAction(UnityEvent unityAction)
    {
        _unityAction = unityAction;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _unityAction.Invoke();
    }
}