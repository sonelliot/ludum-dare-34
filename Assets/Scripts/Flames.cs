using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Simple component that searches it's children for 'Fire' components
/// and activates them after a period of time.
/// </summary>
public class Flames : MonoBehaviour
{
    private List<GameObject> _fires = new List<GameObject>();
    public float startDelay = 5f;

    public void Start()
    {
        foreach (var child in transform)
        {
            var go = (child as Transform).gameObject;
            if (go.name == "Fire")
                _fires.Add(go);
        }

        StartBurning();
    }

    public void StartBurning()
    {
        var delay = 0f;
        foreach (var fire in _fires)
        {
            StartCoroutine(ActivateAfterDelay(fire, delay));
            delay += startDelay;
        }
    }

    private IEnumerator ActivateAfterDelay(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay);
        target.SetActive(true);
        yield return null;
    }
}
