using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("키 입력 잠금")]
    public bool isInputLocked = false;

    void Awake()
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
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LockInput() => isInputLocked = true;
    public void UnLockInput() => isInputLocked = false;
    
}
