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
            _body.isKinematic = true;
            _collider.enabled = false;

            StopAllCoroutines();
            StartCoroutine(CameraShake(2f, 0.05f, 0.5f));
        }
    }

    private IEnumerator CameraShake(float force, float interval, float seconds)
    {
        var cam = Camera.main.GetComponent<CameraController>();
        var sign = 1f;
        var elapsed = 0f;

        while (elapsed < seconds)
        {
            elapsed += Time.deltaTime;
            sign *= -1f;

            var v = cam.velocity;
            cam.velocity = new Vector3(v.x + sign * force, v.y, v.z);

            yield return new WaitForSeconds(interval);
        }

        yield return null;
    }
}
