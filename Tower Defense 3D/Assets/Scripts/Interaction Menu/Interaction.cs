using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Interaction
{
    [SerializeField] Sprite sprite;
    [SerializeField] UnityEvent unityAction;

    public Sprite InteractionSprite => sprite;
    public UnityEvent UnityAction => unityAction;
}
