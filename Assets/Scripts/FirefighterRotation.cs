using UnityEngine;
using System.Collections;

public class FirefighterRotation : MonoBehaviour {

    public Sprite LSprite;
    public Sprite LDSprite;
    public Sprite DSprite;
    public Sprite RDSprite;
    public Sprite RSprite;
    public Sprite RUSprite;
    public Sprite USprite;
    public Sprite LUSprite;

    private SpriteRenderer _SR;

	// Use this for initialization
	void Start () {
        _SR = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
