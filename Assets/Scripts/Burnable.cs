using UnityEngine;
using System.Collections;

public enum BurningState
{
    Unburnt, Burning, Burnt
};

public class Burnable : MonoBehaviour
{
    public float burnRadius = 5;

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
        if (State == BurningState.Burning && _flames.isPlaying == false)
        {
            _flames.Play();

            var hitColliders = Physics.OverlapSphere(transform.position, burnRadius);
            foreach(var collider in hitColliders)
            {
                var burnable = collider.gameObject.GetComponent<Burnable>();
                if (burnable.State == BurningState.Unburnt)
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
        yield return new WaitForSeconds(10f);
        State = BurningState.Burnt;
    }
}
