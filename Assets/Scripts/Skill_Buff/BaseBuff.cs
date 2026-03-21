using System.Collections;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class BaseBuff : MonoBehaviour
{
    public string type; //버프 타입
    public float percentage; //공격력 증가 비율
    public float duration; // 버프 지속 시간
    public float currentTime;
    public UnityEngine.UI.Image icon;
    public Fire fire;

    private PlayerController player;

    private void Awake()
    {
        icon = GetComponent<UnityEngine.UI.Image>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }
    public void Init(string type, float per, float du, Sprite buffSprite) // 버프 정보 설정
    {
        this.type = type;
        percentage = per;
        if(type == "Atk")
        {
            player.atkMultiplier *= (percentage / 100f); // 공격력 증가 식
        }
        else if(type == "SPEED")
        {
            player.speed *= (percentage / 100f);
        }
        duration = du;
        currentTime = duration;
        icon.fillAmount = 1;
        icon.sprite = buffSprite;
        Execute(type);
    }

    WaitForSeconds seconds = new WaitForSeconds(0.1f);
    public void Execute(string type) 
    {
        StartCoroutine(Activation(type));
    }

    IEnumerator Activation(string type) // 버프 아이콘을 남은 시간에 따라 변경
    {
        while(currentTime > 0)
        {
            currentTime -= 0.1f;
            icon.fillAmount = currentTime / duration;
            yield return seconds;
        }

        currentTime = 0;
        DeActivation(type);
    }

    public void DeActivation(string type) // 버프 아이콘 제거
    {
        if(type == "Atk"){
            PlayerAttackSystem.instance.S_Skilling = false;
            player.atkMultiplier *= (100f / percentage);
        }
        else if(type == "SPEED"){
            player.speed *= (100f / percentage);
        }
        Destroy(gameObject);
    }
}
