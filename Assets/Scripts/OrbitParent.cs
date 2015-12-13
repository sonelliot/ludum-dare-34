using UnityEngine;
using System.Collections;

public class OrbitParent : MonoBehaviour
{
    public float speed = 1f;
    public void Update()
    {
        transform.RotateAround(transform.parent.position, Vector3.up,
            speed * Time.deltaTime);
    }
}
