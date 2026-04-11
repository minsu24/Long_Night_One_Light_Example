using UnityEngine;

// 정신력 4단계: 논문 설계 기준
// Normal(70~100), Depression(50~69), Anxiety(30~49), Collapse(0~29)
public enum MentalState { Normal, Depression, Anxiety, Collapse }

public class MentalSystem : MonoBehaviour
{
    private PlayerController player;
    private MentalState currentState = MentalState.Normal;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Start()
    {
        var entity = GameObject.FindGameObjectWithTag("Player").GetComponent<Entity>();
        entity.OnMentalChanged += ChangeMental;
    }

    public void ChangeMental(float currentMental)
    {
        float ratio = currentMental / player.maxMental;
        MentalState newState;

        if (ratio > 0.7f)       newState = MentalState.Normal;
        else if (ratio > 0.5f)  newState = MentalState.Depression;
        else if (ratio > 0.3f)  newState = MentalState.Anxiety;
        else                    newState = MentalState.Collapse;

        ApplyMentalEffect(newState);

        // 상태가 변경된 경우에만 GameManager에 알림 (이벤트 발화 최소화)
        if (newState != currentState)
        {
            currentState = newState;
            GameManager.instance.SetMentalState(currentState);
        }
    }

    private void ApplyMentalEffect(MentalState state)
    {
        switch (state)
        {
            case MentalState.Normal:
                player.speedMultiplier = 1f;
                player.atkMultiplier = 1f;
                break;
            case MentalState.Depression:
                // 우울: 공격력 소폭 증가 (분노 반응)
                player.atkMultiplier = 1.15f;
                break;
            case MentalState.Anxiety:
                // 불안: 이동 속도 소폭 증가 (도주 반응)
                player.speedMultiplier = 1.1f;
                break;
            case MentalState.Collapse:
                // 붕괴: 공격력 대폭 증가 (광기 반응)
                player.atkMultiplier = 1.3f;
                break;
        }
    }
}
