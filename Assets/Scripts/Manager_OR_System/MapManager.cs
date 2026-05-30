using UnityEngine;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;
    public bool isMapOpen = false;

    [SerializeField] private GameObject _largeMap;

    public bool IsLargeMapOpen {get; private set; }


    [System.Serializable]
        public struct MapZoneData
        {
            public string zoneName;       // 구역 이름 (예: "Forest_A", "Desert_B")
            public Vector2 uiTargetPos;   // 이 구역일 때 이미지가 이동할 UI 좌표 (X, Y)
        }

        [Header("Map Settings")]
        public RectTransform movingImage;  // 이동시키고 싶은 특정 이미지 (플레이어 핀 또는 맵 배경)
        public List<MapZoneData> zoneList; // 인스펙터에서 설정할 구역 리스트

        [Header("Current State")]
        public string currentZoneName;    // 현재 플레이어가 위치한 구역 이름

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        CloseLargeMap();
    }

    void Start()
    {
        FindAnyObjectByType<MapManager>().currentZoneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!IsLargeMapOpen)
            {
                OpenLargeMap();
                
                GameManager.instance.LockInput();

            }
            else
            {
                CloseLargeMap();
                GameManager.instance.UnLockInput();
            }
        }
    }
    private void OpenLargeMap()
    {
        IsLargeMapOpen = true;
        _largeMap.SetActive(true);
        Time.timeScale = 0f;
        UpdateMapPosition();
    }
    private void CloseLargeMap()
    {
        IsLargeMapOpen = false;
        _largeMap.SetActive(false);
        Time.timeScale = 1f;
    }

    public void UpdateMapPosition()
    {
        // 안전장치: 이동할 이미지가 할당되지 않았다면 에러 방지
        if (movingImage == null)
        {
            Debug.LogError("MapManager: 'movingImage'가 인스펙터에서 할당되지 않았습니다!");
            return;
        }

        // 현재 구역 이름과 일치하는 데이터를 리스트에서 검색
        MapZoneData targetData = zoneList.Find(x => x.zoneName == currentZoneName);

        // 일치하는 구역을 찾았다면 해당 UI 좌표로 이미지 이동
        if (!string.IsNullOrEmpty(targetData.zoneName))
        {
            movingImage.anchoredPosition = targetData.uiTargetPos;
        }
        else
        {
            Debug.LogWarning($"MapManager: '{currentZoneName}'에 해당하는 맵 좌표 데이터가 없습니다.");
        }
    }
}
