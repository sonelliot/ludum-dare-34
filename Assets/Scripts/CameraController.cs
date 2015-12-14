using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CameraController : MonoBehaviour
{
    public Vector3 velocity;
    private Camera _camera;

    public float followTime = 0.15f;
    public GameObject target = null;

    public void Start()
    {
        _camera = GetComponent<Camera>();
    }

    public void Update()
    {
        if (this.target == null)
            return;

        var position = target.transform.position;
        var point = _camera.WorldToViewportPoint(this.target.transform.position);
        var delta = position - _camera.ViewportToWorldPoint(
            new Vector3(0.5f, 0.5f, point.z));

        var destination = new Vector3(
            transform.position.x + delta.x,
            transform.position.y,
            transform.position.z + delta.z);

        transform.position = Vector3.SmoothDamp(transform.position,
            destination, ref this.velocity, this.followTime);
    }
}
