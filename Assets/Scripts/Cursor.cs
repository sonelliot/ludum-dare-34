using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Cursor : MonoBehaviour
{
    private Camera _camera;
    private IEnumerable<ParticleSystem> _particles;

    public float heatTransfer = 1f;
    public float heatRadius = 1.8f;

    public void Start()
    {
        _camera = Camera.main;
        _particles = GetComponentsInChildren<ParticleSystem>();
    }

    private void ActivateFlames(bool on)
    {
        foreach (var ps in _particles)
        {
            if (!ps.isPlaying && on)
                ps.Play();
            if (ps.isPlaying && !on)
                ps.Stop();
        }
    }

    private void UpdatePosition()
    {
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 8))
        {
            transform.position = new Vector3(
                hit.point.x, transform.position.y, hit.point.z);
        }
    }

    private void UpdateHeat()
    {
        var burnables = Physics.OverlapSphere(transform.position, heatRadius)
            .Select(c => c.GetComponent<NewBurnable>())
            .Where(b => b != null);

        if (!burnables.Any() || !burnables.Any(b => b.IsBurning))
            return;

        foreach (var burnable in burnables)
        {
            var amount = this.heatTransfer * Time.deltaTime;
            burnable.Heat += amount;
            burnable.Wet -= amount;
        }
    }

    public void Update()
    {
        UpdatePosition();

        var leftDown = Input.GetMouseButton(0);
        ActivateFlames(leftDown);

        if (leftDown)
            UpdateHeat();
    }
}
