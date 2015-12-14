﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Burnable : MonoBehaviour
{
    public static List<Burnable> all = new List<Burnable>();

    public static Burnable NearestBurning(Vector3 position, float radius = 0f)
    {
        var burning = all.Where(b => b.IsBurning);
        if (radius > 0f)
        {
            burning = burning.Where(b =>
                Vector3.Distance(position, b.transform.position) < radius);
        }

        if (!burning.Any())
        {
            return null;
        }

        return burning.Aggregate((nearest, candidate) =>
            {
                var d1 = Vector3.Distance(position, nearest.transform.position);
                var d2 = Vector3.Distance(position, candidate.transform.position);
                if (d1 < d2)
                    return nearest;
                return candidate;
            });
    }

    public static bool NoneBurning()
    {
        return !all.Any(b => b.IsBurning);
    }

    private Fire _fire;
    private SpriteRenderer _renderer;
    private Color _color;
    private Player _player;

    [SerializeField]
    private float _health = 100f;
    [SerializeField]
    private float _maxHealth = 100f;
    [SerializeField]
    private float _burningThreshold = 100f;
    [SerializeField]
    private float _burningRadius = 1f;
    [SerializeField]
    private float _flammability = 1f;
    [SerializeField]
    private float _absorption = 50f;
    [SerializeField]
    private float _heat = 0f;
    [SerializeField]
    private float _wet = 0f;

    public float value = 1f;

    public float Heat
    {
        get { return _heat; }
        set { _heat = Mathf.Max(0f, value); }
    }

    public float Wet
    {
        get { return _wet; }
        set { _wet = Mathf.Clamp(value, 0f, _absorption); }
    }

    public bool IsBurning
    {
        get { return IsBurnt == false && Heat > _burningThreshold; }
    }

    public bool IsBurnt
    {
        get { return _health <= 0f; }
    }

    public float BurnPercent
    {
        get { return 1f - _health / _maxHealth; }
    }

    public float BurnStrength
    {
        get { return BurnPercent; }
    }

    public void Start()
    {
        Burnable.all.Add(this);

        _fire = GetComponent<Fire>();
        _renderer = GetComponent<SpriteRenderer>();
        _color = _renderer.color;
        _health = _maxHealth;
        _player = GameObject.Find("Player").GetComponent<Player>();
    }

    public void Update()
    {
        // Change the colour of the object based on remaining health.
        _renderer.color = Color.Lerp(_color, Color.black, BurnPercent);

        // Heat is reduced by any wetness on the burnable every tick.
        Heat -= Wet * Time.deltaTime;

        // If we are burning then the heat will continue to increase over time
        // without player interaction. Additionally, health will decrease and
        // heat will transfer to other objects.
        if (IsBurning == true)
        {
            Heat += _flammability * Time.deltaTime;

            _health = HealthChange(_health, _maxHealth, Heat);
            UpdateHeat();

            // When the burnable has been burnt then update the destruction total.
            if (IsBurnt)
            {
                _player.Destruction += this.value;
            }
        }

        UpdateFire();
    }

    private void UpdateFire()
    {
        if (IsBurning == true && _fire.burning == false)
        {
            _fire.StartBurning();
        }
        else if (IsBurning == false && _fire.burning == true)
        {
            _fire.StopBurning();
        }
    }

    public void UpdateHeat()
    {
        var colliders = Physics.OverlapSphere(transform.position, _burningRadius);
        foreach (var collider in colliders)
        {
            var burnable = collider.GetComponent<Burnable>();
            if (burnable != null)
            {
                burnable.Heat += _flammability * Time.deltaTime;
            }
        }
    }

    private static float HealthChange(float health, float maxHealth, float heat)
    {
        return Mathf.Clamp(health - (heat * Time.deltaTime) / 100f, 0f, maxHealth);
    }
}
