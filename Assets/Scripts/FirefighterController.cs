using UnityEngine;
using System.Collections;
using System.Linq;

public enum State
{
    Searching, Extinguishing
}

public class FirefighterController : MonoBehaviour {

    private GameObject firehose;
    private ParticleSystem waterJet;
    private Rigidbody rb;
    private State _state;

    private GameObject closest;

    public float speed = 3f;
    public float searchRadius = 4f;

	// Use this for initialization
	void Start ()
    {
        rb = GetComponent<Rigidbody>();
        firehose = transform.Find("Firehose").gameObject;
        waterJet = firehose.GetComponent<ParticleSystem>();
        Transition(State.Searching);
    }
	
	// Update is called once per frame
	void Update ()
    {
        switch (_state)
        {
            case State.Searching:
                Searching();
                break;
            case State.Extinguishing:
                Extinguishing();
                break;
        }
	}

    private void Searching()
    {
        Debug.Log("Searching");

        closest = FindClosestBurning();
        if(closest != null)
        {
            Transition(State.Extinguishing);
        }
        else
        {
            waterJet.Stop();
            Stop();
        }
    }

    private void Extinguishing()
    {
        Debug.Log("Extinguishing");

        MoveUpTo(closest.transform, 3f);
        SplashAttack(closest);
    }

    private void Transition(State state)
    {
        _state = state;
    }

    // Find the closest burning object
    private GameObject FindClosestBurning()
    {
        var burnables = Physics.OverlapSphere(transform.position, searchRadius)
            .Where(c => c.gameObject.GetComponent<Burnable>() != null)
            .Where(c => c.gameObject.GetComponent<Burnable>().State == BurningState.Burning);

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
        // If bruning
        if(burnable.GetComponent<Burnable>().State.ToString() == "Burning")
        {
            // If in range
            if (Vector3.Distance(transform.position, burnable.transform.position) >= 0F &&
                Vector3.Distance(transform.position, burnable.transform.position) <= 5F)
            {
                firehose.transform.LookAt(burnable.transform);

                if (!waterJet.isPlaying)
                    waterJet.Play();

                StartCoroutine("Extinguish", burnable);
            }
            else
            {
                if (waterJet.isPlaying)
                    waterJet.Stop();
            }
        }    
        else
        {
            waterJet.Stop();
        }  
    }

    // Quickly dampens the velocity of the firefighter to fake momentum
    private void Stop()
    {
        rb.velocity = rb.velocity * 0.8F;
    }

    private IEnumerator Extinguish(GameObject burningObj)
    {
        yield return new WaitForSeconds(2f);
        burningObj.GetComponent<Burnable>().State = BurningState.Retardant;
        Transition(State.Searching);
    }
}
