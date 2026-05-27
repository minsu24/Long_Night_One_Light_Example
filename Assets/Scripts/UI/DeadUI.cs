using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadUI : MonoBehaviour
{
    public static DeadUI Instance;
    public GameObject deadPanel;
    PlayerController playerController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // 1. 만약 Instance가 비어있다면 (게임 시작 후 처음 생성된 것이라면)
        if (Instance == null)
        {
            Instance = this;
            // 씬이 바뀌어도 이 오브젝트(Canvas)를 파괴하지 않고 유지합니다!
            DontDestroyOnLoad(this.gameObject); 
        }
        else
        {
            // 2. 이미 Instance가 있는데 씬을 이동해서 새로운 Canvas가 또 생겼다면?
            // 중복 방지를 위해 새로 생긴 것을 파괴합니다.
            Destroy(this.gameObject);
        }
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    public void OpenDeadUI()
    {
        deadPanel.SetActive(true);
    }
    void CloseDeadUI()
    {
        deadPanel.SetActive(false);
    }

    public void RestartGame()
    {
        Debug.Log("재시작 버튼 눌림");
        CloseDeadUI();
        Time.timeScale = 1;
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
        playerController.ResetPlayerData();
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
            // 유니티 에디터에서 실행 중일 때 재생(Play) 모드를 끕니다.
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // 실제 빌드된 게임(PC, 모바일 등)을 종료합니다.
            Application.Quit();
        #endif
    }
}
