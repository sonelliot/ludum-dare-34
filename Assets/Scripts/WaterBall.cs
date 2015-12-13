using UnityEngine;
using System.Collections;

public class WaterBall : MonoBehaviour
{
    private ParticleSystem _particles;

    public float radius = 1.8f;
    public float hydration = 10f;
    public bool spraying = false;

    public void Start()
    {
        _particles = GetComponent<ParticleSystem>();
    }

    public void Update()
    {
        if (this.spraying == true && _particles.isPlaying == false)
            _particles.Play();
        if (this.spraying == false && _particles.isPlaying == true)
            _particles.Stop();
        if (this.spraying)
            UpdateWetness();
    }

    public void UpdateWetness()
    {
        var colliders = Physics.OverlapSphere(transform.position, this.radius);
        foreach (var collider in colliders)
        {
            var burnable = collider.GetComponent<NewBurnable>();
            if (burnable != null)
            {
                burnable.Wet += this.hydration * Time.deltaTime;
            }
        }
    }
}
