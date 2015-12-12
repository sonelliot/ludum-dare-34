﻿using UnityEngine;
using System.Collections;

public enum BurningState
{
    Unburnt, Burning, Burnt
};

public class Burnable : MonoBehaviour
{
    [SerializeField] private float health = 10f;
    public float burnRate = 0.5f;
    public float burnRadius = 5;

    private bool ticked = false;

    public BurningState State = BurningState.Unburnt;

    private ParticleSystem _flames;
    private SphereCollider _trigger;

	void Start()
    {
        _flames = transform.Find("Flames").gameObject
            .GetComponent<ParticleSystem>();
        _flames.Stop();

        _trigger = GetComponent<SphereCollider>();

        if (State == BurningState.Burning)
        {
            StartCoroutine("StartBurning");
        }
	}
	
	void Update()
    {
        if(health <= 0)
        {
            State = BurningState.Burnt;
        }

        if (State == BurningState.Burning && !ticked)
        {
            StartCoroutine("BurnTick");
        }

        if (State == BurningState.Burning && _flames.isPlaying == false)
        {
            _flames.Play();

            var hitColliders = Physics.OverlapSphere(transform.position, burnRadius);
            foreach (var collider in hitColliders)
            {
                var burnable = collider.gameObject.GetComponent<Burnable>();
                if (burnable != null && burnable.State == BurningState.Unburnt)
                {
                    collider.SendMessage("StartBurning");
                }
            }
        }
        else if (State == BurningState.Burnt && _flames.isPlaying == true)
        {
            _flames.Stop();
        }
	}

    IEnumerator StartBurning()
    {
        yield return new WaitForSeconds(5f);
        State = BurningState.Burning;
    }

    IEnumerator BurnTick()
    {
        ticked = true;
        health -= burnRate;
        yield return new WaitForSeconds(1f);
        ticked = false;
    }
}
