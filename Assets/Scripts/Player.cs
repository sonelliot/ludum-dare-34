using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Player : MonoBehaviour
{
    private Bar _fireBar;
    private Bar _destructionBar;

    [SerializeField]
    private PauseScreen _pauseScreen;

    [SerializeField]
    private float _fire = 100f;
    [SerializeField]
    private float _fireLimit = 100f;
    [SerializeField]
    private float _fireRegen = 5f;
    [SerializeField]
    private float _destruction = 0f;
    [SerializeField]
    private float _destructionGoal = 100f;

    public float Fire
    {
        get { return _fire; }
        set { _fire = Mathf.Clamp(value, 0f, _fireLimit); }
    }

    public float Destruction
    {
        get { return _destruction; }
        set { _destruction = Mathf.Clamp(value, 0f, _destructionGoal); }
    }

    public void Start()
    {
        _fireBar = GameObject.Find("FireBar").GetComponent<Bar>();
        _destructionBar = GameObject.Find("DestructionBar").GetComponent<Bar>();
    }

    public void FireRegen()
    {
        Fire += _fireRegen * Time.deltaTime;
    }

    public void Update()
    {
        _fireBar.Fill = _fire / _fireLimit;
        _destructionBar.Fill = _destruction / _destructionGoal;

        if (Burnable.NoneBurning())
        {
            _pauseScreen.gameObject.SetActive(true);
            _pauseScreen.Lose();
            _pauseScreen.title.text = "Your fire is no more!";
        }
        else if (_destruction >= _destructionGoal)
        {
            _pauseScreen.gameObject.SetActive(true);
            _pauseScreen.Win();
            _pauseScreen.title.text = "You burnt all the things!";
        }
    }

    public void OnLevelWasLoaded(int n)
    {
        Burnable.all.Clear();
    }
}
