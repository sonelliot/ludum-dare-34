using UnityEngine;
using System.Collections;

public class Burnable : MonoBehaviour
{
    enum State
    {
        Unburnt, Burning, Burnt
    };

    public float burnRadius = 5;

    [SerializeField]
    private State _state = State.Unburnt;
    private ParticleSystem _flames;
    private SphereCollider _Trigger;

	void Start()
    {
        _flames = transform.Find("Flames").gameObject
            .GetComponent<ParticleSystem>();
        _flames.Stop();
        _Trigger = GetComponent<SphereCollider>();
	}
	
	void Update()
    {
        if (_state == State.Burning && _flames.isPlaying == false)
        {
            _flames.Play();
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, burnRadius);
            int i = 0;
            while (i < hitColliders.Length-1)
            {
                hitColliders[i].SendMessage("StartBurning");
                i++;
            }
        }
        else if (_state == State.Burnt && _flames.isPlaying == true)
        {
            _flames.Stop();
        }
	}

    IEnumerator StartBurning()
    {
        yield return new WaitForSeconds(3f);
        _state = State.Burning;
    }
}
