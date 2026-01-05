using UnityEngine;

public class BuffManager : MonoBehaviour
{
    public static BuffManager instance;
    private void Awake()
    {
        instance = this;
    }
    public GameObject buffPrefab;

    public void CreateBuff(string type, float per, float du, Sprite icon) // 버프 아이콘 생성
    {
        GameObject go = Instantiate(buffPrefab, transform);
        go.GetComponent<BaseBuff>().Init(type, per, du, icon);
    }
}
