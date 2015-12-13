using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CameraController : MonoBehaviour
{
    private Vector3 _velocity;
    private Camera _camera;

    public float followTime = 0.15f;

    public void Start()
    {
        _camera = GetComponent<Camera>();
    }

    public void Update()
    {
        var mouseVP = _camera.ScreenToViewportPoint(Input.mousePosition);
        var delta = new Vector3(mouseVP.x - 0.5f, 0f, mouseVP.y - 0.5f);
        if (delta.magnitude < 0.15f)
            delta = Vector3.zero;

        var dest = transform.position + delta.normalized;

        transform.position = Vector3.SmoothDamp(transform.position,
            dest, ref _velocity, followTime);
    }
}
