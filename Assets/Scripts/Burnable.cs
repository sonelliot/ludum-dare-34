using UnityEngine;
using System.Collections;

public enum BurningState
{
    NotBurning, Burning, Burnt, Retardant
};

public class Burnable : MonoBehaviour
{
    [SerializeField] private float health = 10f;
    public float burnRate = 0.5f;
    public float burnRadius = 5;
    public Color burnColor;

    private Color initColor;
    private float maxHealth;
    private bool ticked = false;
    public BurningState State = BurningState.NotBurning;

    private SpriteRenderer _SR;
    private SphereCollider _trigger;
    private Fire _flames;

	void Start()
    {
        _flames = GetComponent<Fire>();
        _trigger = GetComponent<SphereCollider>();
        _SR = GetComponent<SpriteRenderer>();

        initColor = _SR.color;
        maxHealth = health;

        if (State == BurningState.Burning)
        {
            StartCoroutine("StartBurning");
        }
	}

	void Update()
    {
        _SR.color = Color.Lerp(burnColor, initColor, health / maxHealth);
        if(health <= 0)
        {
            State = BurningState.Burnt;
            GetComponent<SpriteRenderer>().color = burnColor;
        }

        if (State == BurningState.Burning && !ticked)
        {
            StartCoroutine("BurnTick");
        }

        if (State == BurningState.Burning && _flames.burning == false)
        {
            _flames.StartBurning();

            var hitColliders = Physics.OverlapSphere(transform.position, burnRadius);
            foreach (var collider in hitColliders)
            {
                var burnable = collider.gameObject.GetComponent<Burnable>();
                if (burnable != null && burnable.State == BurningState.NotBurning)
                {
                    collider.SendMessage("StartBurning");
                }
            }
        }
        else if (State == BurningState.Burnt && _flames.burning == true)
        {
            _flames.StopBurning();
        }
        else if (State == BurningState.NotBurning && _flames.burning == true)
        {
            _flames.StopBurning();
        }

        else if (State == BurningState.Retardant && _flames.burning == true)
        {
            _flames.StopBurning();

            StartCoroutine("Drying");
        }
	}

    IEnumerator StartBurning()
    {
        yield return new WaitForSeconds(1f);
        if (!(State == BurningState.Retardant))
        {
            State = BurningState.Burning;
        }
    }

    IEnumerator BurnTick()
    {
        ticked = true;
        health -= burnRate;
        yield return new WaitForSeconds(1f);
        ticked = false;
    }

    IEnumerator Drying()
    {
        yield return new WaitForSeconds(10f);
        State = BurningState.NotBurning;
    }
}
