using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Firefighter : MonoBehaviour
{
    public enum State
    {
        Searching
    }

    private Rigidbody _body;
    private State _state = State.Searching;
    private Dictionary<State, Func<State, State>> _states;

    [SerializeField]
    private float _speed = 5f;

    public void Start()
    {
        _body = GetComponent<Rigidbody>();
        _states = new Dictionary<State,Func<State, State>>();
        _states[State.Searching] = this.Searching;
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

        MoveTo(nearest.transform.position, 2f);
        return State.Searching;
    }

    private void MoveTo(Vector3 target, float distance = 0f)
    {
        if (Vector3.Distance(target, transform.position) > distance)
        {
            var direction  = (target - transform.position).normalized;
            _body.velocity = direction * _speed;
        }
        else
        {
            _body.velocity *= 0.8f;
        }
    }
}
