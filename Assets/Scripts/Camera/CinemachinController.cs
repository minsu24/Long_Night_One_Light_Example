using Unity.Cinemachine;
using UnityEngine;

public class CinemachinController : MonoBehaviour
{
    public CinemachineCamera cinemachineCamera;
    private GameObject player;

    void Start()
    {   
        cinemachineCamera = GetComponent<CinemachineCamera>();
        player = GameObject.FindGameObjectWithTag("Player");
        cinemachineCamera.Follow = player.transform;
        cinemachineCamera.LookAt = player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
