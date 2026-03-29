using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    [SerializeField]
    private GameObject rawImage;
    public GameObject pressAnyKeyUI;
    bool videoEnd = false;
    void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
    }

    void Update()
    {
        if(videoEnd && Input.anyKeyDown)
        {
            ResumeGame();
        }
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
        if(pressAnyKeyUI != null) pressAnyKeyUI.SetActive(false);
    }
    
    void OnVideoFinished(VideoPlayer source)
    {
        rawImage.SetActive(false);
        if(pressAnyKeyUI != null) pressAnyKeyUI.SetActive(true);
        videoEnd = true;
    }
}
