using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Transform menu;
    [SerializeField] private Transform rules;
    [SerializeField] private Transform author;

    private float menuWidth;

    private void Start()
    {
        menuWidth = menu.GetComponent<RectTransform>().rect.width;
    }

    public void OnEnableMenu()
    {
        menu.localPosition = new Vector2(-Screen.width - menuWidth, 0);
        menu.LeanMoveLocalX(-900, 0.5f).setEaseOutExpo().delay = 0.1f;
    }

    public void CloseMenu()
    {
        menu.LeanMoveLocalX(-Screen.width - menuWidth, 0.5f).setEaseOutExpo();
    }

    public void OnEnableRules()
    {
        rules.localPosition = new Vector2(-Screen.width - rules.GetComponent<RectTransform>().rect.width, 0);
        rules.LeanMoveLocalX(-900, 0.5f).setEaseOutExpo().delay = 0.1f;
    }

    public void CloseRules()
    {
        rules.LeanMoveLocalX(-Screen.width - rules.GetComponent<RectTransform>().rect.width, 0.5f).setEaseOutExpo();
    }

    public void OnEnableAuthor()
    {
        author.localPosition = new Vector2(-Screen.width - author.GetComponent<RectTransform>().rect.width, 0);
        author.LeanMoveLocalX(-900, 0.5f).setEaseOutExpo().delay = 0.1f;
    }

    public void CloseAuthor()
    {
        author.LeanMoveLocalX(-Screen.width - author.GetComponent<RectTransform>().rect.width, 0.5f).setEaseOutExpo();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("CutSceneStart");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}