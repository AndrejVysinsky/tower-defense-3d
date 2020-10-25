using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Interaction
{
    [SerializeField] Sprite interactionSprite;
    [SerializeField] string interactionName;

    public Sprite InteractionSprite => interactionSprite;
    public string InteractionName => interactionName;
    public List<UnityAction> InteractionActions { get; set; } = new List<UnityAction>();
}