using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Simple component that searches it's children for 'Fire' components
/// and activates them after a period of time.
/// </summary>
public class Fire : MonoBehaviour
{
    private List<GameObject> _flames = new List<GameObject>();

    public float startDelay = 5f;
    public float strength = 0.1f;
    public bool burning = false;

    public void Start()
    {
        foreach (var child in transform)
        {
            var go = (child as Transform).gameObject;
            if (go.name == "Flame")
                _flames.Add(go);
        }
    }

    public void StartBurning()
    {
        this.burning = true;

        var delay = 0f;
        foreach (var fire in _flames)
        {
            StartCoroutine(ActivateAfterDelay(fire, delay));
            delay += startDelay;
        }
    }

    public void StopBurning()
    {
        this.burning = false;

        StopAllCoroutines();
        foreach (var fire in _flames)
            fire.SetActive(false);
    }

    private IEnumerator ActivateAfterDelay(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay);
        target.SetActive(true);
        yield return null;
    }
}
