using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CheckpointConnector : MonoBehaviour
{
    [SerializeField] SpriteRenderer sprite;

    public void ShowConnector()
    {
        sprite.enabled = true;
    }

    public void HideConnector()
    {
        sprite.enabled = false;
    }
}