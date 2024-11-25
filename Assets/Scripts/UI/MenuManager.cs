using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Transform menu;
    [SerializeField] private Transform rules;
    [SerializeField] private Transform author;
    [SerializeField] private Transform settings; // ƒобавл€ем новую панель
    private float menuWidth;

    private void Start()
    {
        menuWidth = menu.GetComponent<RectTransform>().rect.width;
    }

    // ƒобавл€ем универсальные методы, которые делают в точности то же самое
    private void EnablePanelWithMenuWidth(Transform panel, float width)
    {
        panel.localPosition = new Vector2(-Screen.width - width, 0);
        panel.LeanMoveLocalX(-900, 0.5f).setEaseOutExpo().delay = 0.1f;
    }

    private void EnablePanelWithOwnWidth(Transform panel)
    {
        panel.localPosition = new Vector2(-Screen.width - panel.GetComponent<RectTransform>().rect.width, 0);
        panel.LeanMoveLocalX(-900, 0.5f).setEaseOutExpo().delay = 0.1f;
    }

    private void ClosePanelWithMenuWidth(Transform panel, float width)
    {
        panel.LeanMoveLocalX(-Screen.width - width, 0.5f).setEaseOutExpo();
    }

    private void ClosePanelWithOwnWidth(Transform panel)
    {
        panel.LeanMoveLocalX(-Screen.width - panel.GetComponent<RectTransform>().rect.width, 0.5f).setEaseOutExpo();
    }

    // ќставл€ем оригинальные методы без изменений
    public void OnEnableMenu()
    {
        EnablePanelWithMenuWidth(menu, menuWidth);
    }

    public void CloseMenu()
    {
        ClosePanelWithMenuWidth(menu, menuWidth);
    }

    public void OnEnableRules()
    {
        EnablePanelWithOwnWidth(rules);
    }

    public void CloseRules()
    {
        ClosePanelWithOwnWidth(rules);
    }

    public void OnEnableAuthor()
    {
        EnablePanelWithOwnWidth(author);
    }

    public void CloseAuthor()
    {
        ClosePanelWithOwnWidth(author);
    }

    // ƒобавл€ем новые методы, использу€ те же универсальные методы
    public void OnEnableSettings()
    {
        EnablePanelWithOwnWidth(settings);
    }

    public void CloseSettings()
    {
        ClosePanelWithOwnWidth(settings);
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