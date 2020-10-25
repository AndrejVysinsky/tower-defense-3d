using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InteractionList : MonoBehaviour
{
    [SerializeField] List<Interaction> interactions;

    public List<Interaction> Interactions => interactions;
}