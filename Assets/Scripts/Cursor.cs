using UnityEngine;
using System.Collections;

public class Cursor : MonoBehaviour
{
    private Camera _camera;

    public void Start()
    {
        _camera = Camera.main;
    }

    public void Update()
    {
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            transform.position = new Vector3(
                hit.point.x, transform.position.y, hit.point.z);
        }
    }
}
