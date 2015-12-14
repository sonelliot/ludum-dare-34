using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Flame : MonoBehaviour
{
    private Vector3 _originalScale;
    private ParticleSystem _particles;

    public float strength = 1f;

    public void Start()
    {
        _originalScale = transform.localScale;
        _particles = GetComponent<ParticleSystem>();
    }

    public void Update()
    {
        if (this.strength > 0f)
        {
            transform.localScale = _originalScale * this.strength;
        }
    }
}
