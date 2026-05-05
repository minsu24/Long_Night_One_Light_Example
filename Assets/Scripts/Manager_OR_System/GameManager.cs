using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("키 입력 잠금")]
    public bool isInputLocked = false;

    [Header("정신력 상태 (읽기 전용 - MentalSystem이 설정)")]
    [SerializeField] public MentalState _currentMentalState = MentalState.Normal;

    // 현재 정신력 상태 (다른 시스템이 읽어가는 중앙 데이터)
    public MentalState CurrentMentalState => _currentMentalState;

    // 상태 전환 시 발화되는 이벤트 (DialogueManager, EnemySpawner 등이 구독)
    public event Action<MentalState> OnMentalStateChanged;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LockInput() => isInputLocked = true;
    public void UnLockInput() => isInputLocked = false;

    // MentalSystem이 상태 전환 시 호출
    public void SetMentalState(MentalState newState)
    {
        _currentMentalState = newState;
        OnMentalStateChanged?.Invoke(newState);
    }
}
