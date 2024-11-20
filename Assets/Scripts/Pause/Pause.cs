using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    [SerializeField] private Transform menu;
    //public Rigidbody playerRigidbody;
    public bool isRPaused = false;

    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Escape))
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

    public void PauseScreen()
    {
        menu.localPosition = new Vector2(-Screen.width, 0);
        menu.LeanMoveLocalX(0, 0.5f).setEaseOutExpo().delay = 0.1f;
        //playerRigidbody.isKinematic = true;
        isRPaused = true;
    }

    public void Play()
    {
        isRPaused = false;
        menu.LeanMoveLocalX(-Screen.width, 0.5f).setEaseOutExpo();
        //playerRigidbody.isKinematic = false;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}

