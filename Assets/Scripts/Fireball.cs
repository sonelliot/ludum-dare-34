using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Fireball : MonoBehaviour
{
    private Rigidbody _body;
    private SphereCollider _collider;

    public float heatRadius = 2f;
    public float heatTransfer = 1f;

    public void Awake()
    {
        _body = GetComponent<Rigidbody>();
        _collider = GetComponent<SphereCollider>();
    }

    public void Update()
    {
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Ground")
        {
            Debug.Log("hit ground");

            _body.isKinematic = true;
            _collider.enabled = false;
        }
    }
}
