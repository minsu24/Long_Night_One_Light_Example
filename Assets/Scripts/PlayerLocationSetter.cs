using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLocationSetter : MonoBehaviour
{

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
            if(spawnPoint != null)
            {
                var controller = GetComponent<PlayerController>();
                if(controller != null) controller.enabled = false;
                Debug.Log("스폰 위치 확인");
                transform.position = spawnPoint.transform.position;
                transform.rotation = spawnPoint.transform.rotation;
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
