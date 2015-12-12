using UnityEngine;
using System.Collections;
using System.Linq;

public enum State
{
    Searching, Extinguishing
}

public class FirefighterController : MonoBehaviour {

    private Rigidbody rb;
    private State _state;
    private Coroutine _coroutine;

    public float speed = 3f;
    public float searchRadius = 4f;

	// Use this for initialization
	void Start ()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(Transition(State.Searching));
    }
	
	// Update is called once per frame
	void Update ()
    {
	}

    private IEnumerator Searching()
    {
        Debug.Log("Searching");

        var closest = (GameObject)null;
        while(closest == null)
        {
            closest = FindClosestBurnable();
            yield return new WaitForEndOfFrame();
        }

        yield return Transition(State.Extinguishing, closest);
    }

    private IEnumerator Extinguishing(GameObject burnable)
    {
        Debug.Log("Extinguishing");

        while(true)
        {
            MoveUpTo(burnable.transform, 1f);
            SplashAttack(burnable);
            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }

    private IEnumerator Transition(State state, object arg = null)
    {
        _coroutine = StartCoroutine(state.ToString(), arg);
        _state = state;

        yield return null;
    }

    // Find the closest burning object
    private GameObject FindClosestBurnable()
    {
        var burnables = Physics.OverlapSphere(transform.position, searchRadius)
            .Where(c => c.gameObject.GetComponent<Burnable>() != null);

        if (!burnables.Any())
            return null;

        return burnables.Aggregate((closest, consider) =>
            {
                var d1 = Vector3.Distance(closest.transform.position, transform.position);
                var d2 = Vector3.Distance(consider.transform.position, transform.position);
                if (d1 < d2)
                    return closest;
                return consider;
            }).gameObject;
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

     private void SplashAttack(GameObject burnable)
    {
        if (Vector3.Distance(transform.position, burnable.transform.position) >= 0.5F &&
            Vector3.Distance(transform.position, burnable.transform.position) <= 2F)
        {
            Debug.Log("Putting the fire out");
        }
    }

    // Quickly dampens the velocity of the firefighter to fake momentum
    private void Stop()
    {
        rb.velocity = rb.velocity * 0.8F;
    }
}
