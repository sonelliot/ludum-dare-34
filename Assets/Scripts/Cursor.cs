using UnityEngine;
using System.Collections;

public class Cursor : MonoBehaviour
{
    private Camera _camera;

    public float heatTransfer = 1f;
    public float heatRadius = 1.8f;

    public void Start()
    {
        _camera = Camera.main;
    }

    public void UpdatePosition()
    {
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            transform.position = new Vector3(
                hit.point.x, transform.position.y, hit.point.z);
        }
    }

    public void UpdateHeat()
    {
        var colliders = Physics.OverlapSphere(transform.position, heatRadius);
        foreach (var collider in colliders)
        {
            var burnable = collider.GetComponent<NewBurnable>();
            if (burnable != null)
            {
                burnable.heat += this.heatTransfer * Time.deltaTime;
            }
        }
    }

    public void Update()
    {
        UpdatePosition();
        UpdateHeat();
    }
}
