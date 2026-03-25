using UnityEngine;
using System.IO;

public class PlayerData
{
    public string name;
    public float hp;
    public float mp;
    public int mental;
    public int item;
    public string lastPlayTime;
}

public class DataManager : MonoBehaviour
{

    public static DataManager instance;
    public string path;
    public int nowSlot;
    public PlayerData nowPlayer = new PlayerData();
    void Awake()
    {
        #region 싱글톤
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(instance.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        #endregion
        path = Application.persistentDataPath + "/save";
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GameSave()
    {
        string data = JsonUtility.ToJson(nowPlayer);
        File.WriteAllText(path + nowSlot.ToString(), data);        
    }
    public void GameLoad()
    {
        string data = File.ReadAllText(path + nowSlot.ToString());
        nowPlayer = JsonUtility.FromJson<PlayerData>(data);
    }
    public void DataClear()
    {
        nowSlot = -1;
        nowPlayer = new PlayerData();
    }
}
