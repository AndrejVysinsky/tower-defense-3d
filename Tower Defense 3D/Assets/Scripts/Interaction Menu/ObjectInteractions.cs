using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectInteractions : MonoBehaviour
{
    [SerializeField] List<Interaction> interactions;

    public List<Interaction> Interactions => interactions;

    public void Remove(string name, UnityAction action)
    {
        int before = interactions.Count;

        for (int i = interactions.Count - 1; i >= 0; i--)
        {
            var interactionEvent = interactions[i].UnityAction;

            for (int j = 0; j < interactionEvent.GetPersistentEventCount(); j++)
            {
                if (interactionEvent.GetPersistentMethodName(j) == name)
                {
                    interactions.RemoveAt(i);
                    break;
                }
            }
        }

        int after = interactions.Count;

        if (before != after)
        {
            InteractionSystem.Instance.ShowInteractions(interactions);
        }
    }

    private void OnDestroy()
    {
        InteractionSystem.Instance.HideInteractions();
    }
}