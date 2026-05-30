using UnityEngine;

public class BuffManager : MonoBehaviour
{
    public static BuffManager instance;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 첫 매니저만 파괴되지 않고 유지
        }
        else
        {
            Destroy(gameObject); 
            return;
        }
        
    }
    public GameObject buffPrefab;

    public void CreateBuff(string type, float per, float du, Sprite icon) // 버프 아이콘 생성
    {
        GameObject go = Instantiate(buffPrefab, transform);
        go.GetComponent<BaseBuff>().Init(type, per, du, icon);
    }
}
