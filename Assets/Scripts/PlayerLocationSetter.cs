using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLocationSetter : MonoBehaviour
{
    PlayerController playerController;

    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string targetName = MapTransferData.TargetSpawnPointname;

        if (!string.IsNullOrEmpty(targetName))
        {
            GameObject spawnPoint = GameObject.Find(targetName);
            if (spawnPoint == null)
            {
                Debug.Log("목적지를 못 찾음! 기본 스폰 포인트(Respawn 태그)를 찾습니다.");
                spawnPoint = GameObject.FindGameObjectWithTag("Respawn");
            }
            if(spawnPoint != null)
            {
                var controller = GetComponent<PlayerController>();
                if(controller != null) controller.enabled = false;
                Debug.Log("스폰 위치 확인");
                playerController.rb.position = spawnPoint.transform.position;
                transform.rotation = spawnPoint.transform.rotation;
                playerController.rb.linearVelocity = Vector2.zero;
                if(controller != null) controller.enabled = true;
            }
            else
            {
                Debug.Log("스폰 위치 실종");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
