using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    [SerializeField]
    private GameObject rawImage;

    void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
    }

    void OnEnable()
    {
        PauseGame();
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    void OnDisable()
    {
        videoPlayer.loopPointReached -= OnVideoFinished;
    }
    
    void PauseGame()
    {
        Time.timeScale = 0f;
    }
    void ResumeGame()
    {
        Time.timeScale = 1f;
    }
    
    void OnVideoFinished(VideoPlayer source)
    {
        ResumeGame();
        rawImage.SetActive(false);
    }
}
