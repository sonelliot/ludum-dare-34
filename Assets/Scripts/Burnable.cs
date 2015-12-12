using UnityEngine;
using System.Collections;

public class Burnable : MonoBehaviour
{
    enum State
    {
        Unburnt, Burning, Burnt
    };

    [SerializeField]
    private State _state = State.Unburnt;
    private ParticleSystem _flames;

	void Start()
    {
        _flames = transform.Find("Flames").gameObject
            .GetComponent<ParticleSystem>();
        _flames.Stop();
	}
	
	void Update()
    {
        if (_state == State.Burning && _flames.isPlaying == false)
        {
            _flames.Play();
        }
        else if (_state == State.Burnt && _flames.isPlaying == true)
        {
            _flames.Stop();
        }
	}
}
