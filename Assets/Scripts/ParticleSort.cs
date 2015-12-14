using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ParticleSort : MonoBehaviour
{
    public string layerName;

    public void Awake()
    {
        var renderer = GetComponent<ParticleSystem>().GetComponent<Renderer>();
        renderer.sortingLayerName = this.layerName;
    }
}
