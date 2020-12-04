using System.Collections.Generic;
using UnityEngine;

public class MeshRendererContainer : MonoBehaviour
{
    [SerializeField] List<MeshRenderer> meshRenderers;

    public List<MeshRenderer> MeshRenderers => meshRenderers;
}