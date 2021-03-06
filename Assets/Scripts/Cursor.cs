﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Cursor : MonoBehaviour
{
    private Camera _camera;
    private Player _player;
    private IEnumerable<ParticleSystem> _particles;
    private bool _regenerating = false;

    [SerializeField]
    private GameObject _fireballPrefab;
    [SerializeField]
    private float _fireConsumption = 1f;
    [SerializeField]
    private Gradient _burnGradient;
    private ParticleSystem.MinMaxGradient _burnMinMax;
    [SerializeField]
    private Gradient _normalGradient;
    private ParticleSystem.MinMaxGradient _normalMinMax;
    [SerializeField]
    private float _orbitSpeed = 200f;
    private IEnumerable<OrbitParent> _orbits;

    public float heatTransfer = 1f;
    public float heatRadius = 1.8f;

    public void Start()
    {
        _camera = Camera.main;
        _particles = GetComponentsInChildren<ParticleSystem>();
        _player = GameObject.Find("Player").GetComponent<Player>();

        _burnMinMax = new ParticleSystem.MinMaxGradient(_burnGradient);
        _normalMinMax = new ParticleSystem.MinMaxGradient(_normalGradient);

        _orbits = GetComponentsInChildren<OrbitParent>();
    }

    private void ActivateFlames(bool on)
    {
        foreach (var ps in _particles)
        {
            var grad = on ? _burnMinMax : _normalMinMax;
            var col = ps.colorOverLifetime;
            col.color = grad;
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
            .Select(c => c.GetComponent<Burnable>())
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

    private void UpdateOrbits(bool heating)
    {
        var speed = (heating ? 2f : 1f) * _orbitSpeed;
        foreach (var orbit in _orbits)
        {
            orbit.speed = speed;
        }
    }

    public IEnumerator RegenTimeout(float seconds)
    {
        _regenerating = true;
        yield return new WaitForSeconds(seconds);
        _regenerating = false;
        yield return null;
    }

    public bool CanHeat()
    {
        return !_regenerating && _player.Fire > 0f && Input.GetMouseButton(0);
    }

    public bool CanLaunch()
    {
        return !_regenerating && _player.FireFull && Input.GetMouseButtonDown(1);
    }

    private void LaunchFireball()
    {
        var origin = new Vector3(
            transform.position.x,
            transform.position.y + 10f,
            transform.position.z);

        Instantiate(_fireballPrefab, origin, Quaternion.identity);

        _player.Fire = 0f;
    }

    public void Update()
    {
        UpdatePosition();

        var heating = CanHeat();
        ActivateFlames(heating);

        UpdateOrbits(heating);

        if (CanLaunch())
        {
            LaunchFireball();
        }

        if (heating)
        {
            UpdateHeat();
            _player.Fire -= _fireConsumption * Time.deltaTime;

            if (Mathf.Approximately(_player.Fire, 0f))
            {
                StartCoroutine(RegenTimeout(1f));
            }
        }
        else
        {
            _player.FireRegen();
        }
    }
}
