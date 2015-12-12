using UnityEngine;
using System.Collections;

public enum State
{
    Searching, Extinguishing
}

public class FirefighterController : MonoBehaviour {

    private State _state;

    public float searchRadius = 4f;

	// Use this for initialization
	void Start ()
    {
        _state = State.Searching;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(_state == State.Searching)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, searchRadius);
            Collider closest = FindClosestBurning(hitColliders);
            Debug.Log(closest.transform.position);
        }
	}

    private Collider FindClosestBurning(Collider[] cols)
    {
        Collider closest = new Collider();

        for (int i = 0; i < cols.Length-1; i++)
        {
            if (i == 0)
            {
                closest = cols[i];
            }
            else if(Vector3.Distance(cols[i].transform.position, transform.position) <
                    Vector3.Distance(closest.transform.position, transform.position))
            {
                closest = cols[i];
            }
        }

        return closest;
    }
}
