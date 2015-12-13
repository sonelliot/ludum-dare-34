using UnityEngine;
using System.Collections;

public class Accelerate : MonoBehaviour
{
    public float influenceRadius = 1F;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Collider[] hitColliders = Physics.OverlapSphere(hit.point, influenceRadius);
                foreach (var obj in hitColliders)
                {
                    Debug.Log(obj.name);
                }
            }
        }
    }
}
