using UnityEngine;
using System.Collections;
using System.Linq;

public enum State
{
    Searching, Extinguishing
}

public class FirefighterController : MonoBehaviour {

    private Collider closest = null;
    private Rigidbody rb;
    private State _state;

    public float speed = 3f;
    public float searchRadius = 4f;

	// Use this for initialization
	void Start ()
    {
        rb = GetComponent<Rigidbody>();
        _state = State.Searching;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (_state == State.Searching)
        {
            closest = FindClosest();
            if (closest && closest.GetComponent<Burnable>().State.ToString() == "Burning")
            {
                _state = State.Extinguishing;
            }
        }
        else if (_state == State.Extinguishing)
        {
            if(closest)
            {
                MoveUpTo(closest.transform, 1F);
            }
            else
            {
                Stop();
            }
        }
	}

    // Find the closest burning object
    private Collider FindClosest()
    {
        var burnables = Physics.OverlapSphere(transform.position, searchRadius)
            .Where(c => c.gameObject.tag == "Burnable");

        if (!burnables.Any())
            return null;

        return burnables.Aggregate((closest, consider) =>
            {
                var d1 = Vector3.Distance(closest.transform.position, transform.position);
                var d2 = Vector3.Distance(consider.transform.position, transform.position);
                if (d1 < d2)
                    return closest;
                return consider;
            });
    }

    // Move the firefighter in direction dir
    private void Move(Vector3 dir)
    {
        rb.velocity = dir.normalized * speed;
    }

    // Move the firefighter to an exact spot
    private void MoveTo(Transform target)
    {
        Vector3 dir = target.position - transform.position;
        Move(dir);
    }

    // Move the firefighter up to a spot, useful for stopping in front of burning objects
    private void MoveUpTo(Transform target, float stopRange)
    {
        if(Vector3.Distance(transform.position, target.position) >= stopRange)
        {
            Vector3 dir = target.position - transform.position;
            Move(dir);
        }      
        else
        {
            Stop();
        }
    }

    // Quickly dampens the velocity of the firefighter to fake momentum
    private void Stop()
    {
        rb.velocity = rb.velocity * 0.8F;
    }
}
