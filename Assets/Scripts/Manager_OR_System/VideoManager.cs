using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    [SerializeField]
    private GameObject VideoCanvas;

    [SerializeField]
    private GameObject rawImage;
    public GameObject pressAnyKeyUI;
    public EyeBlinkEffect EyeBilnkEffect;

    bool videoEnd = false;
    void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
    }

    void Start()
    {
        if (GameManager.instance.hasWatchedIntroVideo)
        {
            Destroy(gameObject);
            return;
        }
        GameManager.instance.LockInput();
    }
    void Update()
    {
        if (EyeBilnkEffect.isEnd)
        {
            if(pressAnyKeyUI != null) pressAnyKeyUI.SetActive(true);
            EyeBilnkEffect.isEnd = false;
        }
        if(videoEnd && Input.anyKeyDown)
        {
            StartCoroutine(ResumeWithDelay());
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
        GameManager.instance.hasWatchedIntroVideo = true;
        VideoCanvas.SetActive(false);
        rawImage.SetActive(false);
        videoEnd = true;
        EyeBilnkEffect.WakeUp();
    }

    IEnumerator ResumeWithDelay()
    {
        ResumeGame();
        yield return null;
        GameManager.instance.UnLockInput();
    }
}
