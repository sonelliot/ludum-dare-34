﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Firefighter : MonoBehaviour
{
    public enum State
    {
        Idle, Searching, Extinguishing
    }

    private Rigidbody _body;
    private ParticleSystem _hose;
    private Burnable _target;
    private WaterBall _water;
    private SpriteRenderer _renderer;

    private State _state = State.Idle;
    private Dictionary<State, Func<State, State>> _states;

    [SerializeField]
    private float _speed = 5f;
    [SerializeField]
    private float _hydration = 10f;
    [SerializeField]
    private Sprite[] _sprites;

    public void Start()
    {
        _body = GetComponent<Rigidbody>();
        _hose = GetComponentInChildren<ParticleSystem>();
        _water = GetComponentInChildren<WaterBall>();
        _renderer = transform.Find("Sprite").GetComponent<SpriteRenderer>();

        _states = new Dictionary<State,Func<State, State>>();
        _states[State.Idle]           = this.Idle;
        _states[State.Searching]      = this.Searching;
        _states[State.Extinguishing]  = this.Extinguishing;
    }

    private void UpdateSprite()
    {
        if (_target == null)
            return;

        var dirn = (_target.transform.position - transform.position).normalized;    
        var angle = Vector3.Angle(Vector3.right, dirn);
        if (transform.position.z > _target.transform.position.z)
            angle = 180f + (180f - angle);

        var incr = 360f / _sprites.Length;
        var index = Math.Min(Math.Max(Mathf.RoundToInt(angle / incr), 0), _sprites.Length-1);
        index = (index + 3) % _sprites.Length;

        _renderer.sprite = _sprites[index];
    }

    public void Update()
    {
        var active = _states[_state];
        _state = active(_state);

        UpdateSprite();
    }

    private State Idle(State state)
    {
        Stop();

        var nearest = Burnable.NearestBurning(transform.position);
        if (nearest == null)
        {
            return State.Idle;
        }

        _target = nearest;
        return State.Searching;
    }

    private State Searching(State state)
    {
        if (_target.IsBurning == false)
        {
            _target = null;
            return State.Idle;
        }

        if (WithinRange(_target.transform.position, 2f))
        {
            return State.Extinguishing;
        }
        else
        {
            Move(_target.transform.position);
        }

        return State.Searching;
    }

    private State Extinguishing(State state)
    {
        // Slow to a stop.
        Stop();

        // Activate the hose if it isn't on yet.
        WaterActivate(true);

        if (_target.IsBurning == false)
        {
            WaterActivate(false);
            return State.Searching;
        }

        // Spray the target until it's not burning anymore.
        Spray(_target);

        return State.Extinguishing;
    }

    private void WaterActivate(bool on)
    {
        if (_hose.isPlaying == false && on)
        {
            _hose.Play();
            _water.spraying = true;
        }

        if (_hose.isPlaying == true && !on)
        {
            _hose.Stop();
            _water.spraying = false;
        }
    }

    private void Spray(Burnable target)
    {
        _water.spraying = true;
        _water.transform.position = target.transform.position;

        _hose.transform.LookAt(_target.transform);
        _target.Wet += _hydration * Time.deltaTime;
    }

    private bool WithinRange(Vector3 target, float range)
    {
        return Vector3.Distance(target, transform.position) <= range;
    }

    private void Move(Vector3 target, float distance = 0f)
    {
        var direction  = (target - transform.position).normalized;
        _body.velocity = direction * _speed;
    }

    private void Stop()
    {
        _body.velocity *= 0.9f;
    }
}
