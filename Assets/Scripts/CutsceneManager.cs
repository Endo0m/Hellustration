using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System;

public class CutsceneManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    private bool isSkipped = false;
    [SerializeField] string nameScene;

    void Start()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        videoPlayer.loopPointReached += OnVideoEnd;
        videoPlayer.Play();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SkipCutscene();
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        LoadGameScene();
    }

    void SkipCutscene()
    {
        if (!isSkipped)
        {
            isSkipped = true;
            videoPlayer.Stop();
            LoadGameScene();
        }
    }

    void LoadGameScene()
    {
        var index = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(nameScene);

    }
}