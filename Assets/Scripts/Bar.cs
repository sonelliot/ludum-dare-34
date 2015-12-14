using UnityEngine;
using System.Collections;

public class Bar : MonoBehaviour
{
    private GameObject _image;
    [SerializeField]
    private float _fill = 1f;

    public float Fill
    {
        get { return _fill; }
        set { _fill = Mathf.Clamp(value, 0f, 1f); }
    }

    public void Start()
    {
        _image = transform.Find("Fill").gameObject;
    }

    public void Update()
    {
        _image.transform.localScale = new Vector3(_fill, 1f, 1f);
    }
}
