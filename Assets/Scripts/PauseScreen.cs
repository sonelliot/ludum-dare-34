using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PauseScreen : MonoBehaviour
{
    private GameObject _next;
    private GameObject _prev;
    private GameObject _retry;

    public Text title;

    public void Awake()
    {
        this.title = GetComponentInChildren<Text>();

        _next = transform.Find("NextButton").gameObject;
        _next.GetComponent<Button>().onClick.AddListener(OnNextClick);

        _prev = transform.Find("PrevButton").gameObject;
        _prev.GetComponent<Button>().onClick.AddListener(OnPrevClick);

        _retry = transform.Find("RetryButton").gameObject;
        _retry.GetComponent<Button>().onClick.AddListener(OnRetryClick);
    }

    private bool IsNextScene
    {
        get { return SceneManager.GetActiveScene().buildIndex < 2; }
    }

    private bool IsPrevScene
    {
        get { return SceneManager.GetActiveScene().buildIndex != 0; }
    }

    public void Win()
    {
        _next.SetActive(IsNextScene);
        _prev.SetActive(IsPrevScene);
    }

    public void Lose()
    {
        _next.SetActive(false);
        _prev.SetActive(IsPrevScene);
    }

    private void OnEnable()
    {
        Time.timeScale = 0f;
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }

    private void OnNextClick()
    {
        var active = SceneManager.GetActiveScene();
        SceneManager.UnloadScene(active.name);
        SceneManager.LoadScene(active.buildIndex + 1);
    }

    private void OnPrevClick()
    {
        var active = SceneManager.GetActiveScene();
        SceneManager.UnloadScene(active.name);
        SceneManager.LoadScene(active.buildIndex - 1);
    }

    private void OnRetryClick()
    {
        var active = SceneManager.GetActiveScene();
        SceneManager.UnloadScene(active.name);
        SceneManager.LoadScene(active.name);
    }
}
