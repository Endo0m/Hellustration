using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    [SerializeField] private Transform menu;
    private bool isRPaused = false;
    private bool isHintOpen = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isHintOpen)
        {
            if (isRPaused)
            {
                Play();
            }
            else
            {
                PauseScreen();
            }
        }
    }

    public void SetHintOpen(bool isOpen)
    {
        isHintOpen = isOpen;
    }

    public void PauseScreen()
    {
        Time.timeScale = 0f;
        menu.localPosition = new Vector2(-Screen.width, 0);
        menu.LeanMoveLocalX(0, 0.5f)
          .setEaseOutExpo()
          .setDelay(0.1f)
          .setIgnoreTimeScale(true);
        isRPaused = true;
    }

    public void Play()
    {
        Time.timeScale = 1f;
        isRPaused = false;
        menu.LeanMoveLocalX(-Screen.width, 0.5f).setEaseOutExpo();

    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(0);
    }
}

