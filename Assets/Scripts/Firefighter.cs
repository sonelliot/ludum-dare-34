using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Firefighter : MonoBehaviour
{
    public enum State
    {
        Searching, Extinguishing
    }

    private Rigidbody _body;
    private ParticleSystem _hose;
    private NewBurnable _target;

    private State _state = State.Searching;
    private Dictionary<State, Func<State, State>> _states;

    [SerializeField]
    private float _speed = 5f;
    [SerializeField]
    private float _hydration = 10f;

    public void Start()
    {
        _body = GetComponent<Rigidbody>();
        _hose = GetComponentInChildren<ParticleSystem>();

        _states = new Dictionary<State,Func<State, State>>();
        _states[State.Searching]      = this.Searching;
        _states[State.Extinguishing]  = this.Extinguishing;
    }

    public void Update()
    {
        var active = _states[_state];
        _state = active(_state);
    }

    private State Searching(State state)
    {
        var nearest = NewBurnable.NearestBurning(transform.position);
        if (nearest == null)
        {
            return State.Searching;
        }

        _target = nearest;

        if (WithinRange(nearest.transform.position, 2f))
        {
            return State.Extinguishing;
        }
        else
        {
            Move(nearest.transform.position);
        }

        return State.Searching;
    }

    private State Extinguishing(State state)
    {
        // Slow to a stop.
        Stop();
        // Activate the hose if it isn't on yet.
        HoseActivate(true);

        if (_target.IsBurning == false)
        {
            return State.Searching;
        }

        // Spray the target until it's not burning anymore.
        Spray(_target);

        return State.Extinguishing;
    }

    private void HoseActivate(bool on)
    {
        if (_hose.isPlaying == false && on)
            _hose.Play();
        if (_hose.isPlaying == true && !on)
            _hose.Stop();
    }

    private void Spray(NewBurnable target)
    {
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
