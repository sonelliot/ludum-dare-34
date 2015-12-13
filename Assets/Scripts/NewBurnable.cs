using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NewBurnable : MonoBehaviour
{
    public static List<NewBurnable> all = new List<NewBurnable>();

    public static NewBurnable NearestBurning(Vector3 position, float radius = 0f)
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

    private Fire _fire;
    private SpriteRenderer _renderer;
    private Color _color;

    [SerializeField]
    private float _health = 100f;
    [SerializeField]
    private float _maxHealth = 100f;
    [SerializeField]
    public float _burningThreshold = 100f;
    [SerializeField]
    public float _burningRadius = 1f;
    [SerializeField]
    public float _flammability = 1f;

    public float heat = 0f;

    public bool IsBurning
    {
        get { return IsBurnt == false && this.heat > _burningThreshold; }
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
        NewBurnable.all.Add(this);

        _fire = GetComponent<Fire>();
        _renderer = GetComponent<SpriteRenderer>();
        _color = _renderer.color;
        _health = _maxHealth;
    }

    public void Update()
    {
        // Change the colour of the object based on remaining health.
        _renderer.color = Color.Lerp(_color, Color.black, BurnPercent);

        // If we are burning then the heat will continue to increase over time
        // without player interaction. Additionally, health will decrease and
        // heat will transfer to other objects.
        if (IsBurning == true)
        {
            this.heat += _flammability * Time.deltaTime;
            _health = HealthChange(_health, _maxHealth, this.heat);
            UpdateHeat();
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
            var burnable = collider.GetComponent<NewBurnable>();
            if (burnable != null)
            {
                burnable.heat += _flammability * Time.deltaTime;
            }
        }
    }

    private static float HealthChange(float health, float maxHealth, float heat)
    {
        return Mathf.Clamp(health - (heat * Time.deltaTime) / 100f, 0f, maxHealth);
    }
}
