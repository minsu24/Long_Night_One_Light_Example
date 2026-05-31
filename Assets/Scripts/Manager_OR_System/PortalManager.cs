using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalManager : MonoBehaviour
{
    [SerializeField]
    private string sceneName;
    [SerializeField]
    private string targetPointName;
    private string currentSceneName;
    private bool isPlayerInPortal = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isPlayerInPortal && Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveToScene();
        }
    }

    private void MoveToScene()
    {
        // currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        // if(currentSceneName == "BossRoom1")
        // {
        //     return;
        // }
        SceneManager.LoadScene(sceneName);    
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("맵 이동");
            MapTransferData.TargetSpawnPointname = targetPointName;
            isPlayerInPortal = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInPortal = false;
        }
    }

}
