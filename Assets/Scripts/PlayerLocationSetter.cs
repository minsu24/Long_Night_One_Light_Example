using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

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
        FindAnyObjectByType<MapManager>().currentZoneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;


        if (!string.IsNullOrEmpty(targetName))
        {
            GameObject spawnPoint = GameObject.Find(targetName);
            if (spawnPoint == null)
            {
                Debug.Log("목적지를 못 찾음! 기본 스폰 포인트(Respawn 태그)를 찾습니다.");
                spawnPoint = GameObject.FindGameObjectWithTag("Respawn");
            }
            if (spawnPoint != null)
        {
            if (playerController != null) playerController.enabled = false;
            
            Debug.Log("스폰 위치 확인");
            
            // 1. 플레이어 위치 및 물리 초기화
            playerController.rb.position = spawnPoint.transform.position;
            transform.rotation = spawnPoint.transform.rotation;
            playerController.rb.linearVelocity = Vector2.zero;
            playerController.rb.angularVelocity = 0f;
            playerController.rb.Sleep();
            playerController.rb.WakeUp();

            // 2. ★ [핵심] 시네머신 카메라의 위치를 강제로 스폰 포인트로 옮기고 기억 포맷 ★
            // 유니티 6(시네머신 3) 기준 코드입니다. 만약 에러가 나면 아래의 [버전별 컴파일 에러 해결법]을 참고하세요.
            var vCam = FindAnyObjectByType<Unity.Cinemachine.CinemachineCamera>();
            if (vCam != null)
            {
                // 가상 카메라 오브젝트 자체의 위치를 스폰 포인트로 강제 이동
                vCam.transform.position = spawnPoint.transform.position;
                
                // 시네머신 내부의 댐핑/추적 이력을 즉시 파괴하고 이 위치로 고정 (패닝 현상 원천 차단)
                vCam.ForceCameraPosition(spawnPoint.transform.position, spawnPoint.transform.rotation);
            }

            // 3. 실제 화면을 그리는 메인 카메라의 위치도 즉시 이동 (남색 화면 1프레임 노출 방지)
            if (Camera.main != null)
            {
                Vector3 targetCamPos = spawnPoint.transform.position;
                targetCamPos.z = Camera.main.transform.position.z; // Z축은 카메라 원래 수치 유지
                Camera.main.transform.position = targetCamPos;
            }

            if (playerController != null) playerController.enabled = true;
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
