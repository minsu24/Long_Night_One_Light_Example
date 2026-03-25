using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    [SerializeField] private GameObject _largeMap;

    public bool IsLargeMapOpen {get; private set; }

    private void Awake()
    {
        if(instance = null)
        {
            instance = this;
        }

        CloseLargeMap();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!IsLargeMapOpen)
            {
                OpenLargeMap();
            }
            else
            {
                CloseLargeMap();
            }
        }
    }
    private void OpenLargeMap()
    {
        _largeMap.SetActive(true);
        IsLargeMapOpen = true;
    }
    private void CloseLargeMap()
    {
        _largeMap.SetActive(false);
        IsLargeMapOpen = false;
    }
}
