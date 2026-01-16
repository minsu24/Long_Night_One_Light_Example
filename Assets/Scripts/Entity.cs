using System;
using Unity.Mathematics;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    private Stats stats; //캐릭터 정보
    public Entity target; //공격 대상
    
    // 체력(HP) 프로퍼티 : 0 ~ maxHP 사이의 값을 넘어갈 수 없도록 설정
     public float HP
    {
        set => stats.HP = Mathf.Clamp(value, 0, maxHP);
        get => stats.HP;
    } 
    // 마나(MP) 프로퍼티 : 0 ~ maxMP 사이의 값을 넘어갈 수 없도록 설정
    public float MP
    {
        set => stats.MP = Mathf.Clamp(value, 0, maxMP);
        get => stats.MP;
    }

    public float Mental
    {
        set => stats.Mental = Mathf.Clamp(value, 0, maxMental);
        get => stats.Mental;
    }


    public abstract float maxHP { get; }
    public abstract float maxMP { get; }
    public abstract float maxMental { get; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    protected void Setup()
    {
        HP = maxHP;
        MP = maxHP;
        Mental = 0;
    }

    //상대방을 공격할 때 상대방의 TakeDamage() 호출
    // 매개변수 damage는 공격하는 본인 공격력
    public abstract void TakeDamage(float damage);

    [System.Serializable]
    public struct Stats
    {   //이름, 레벨, 스탯, 체력/마나, 정신력 등의 캐릭터 수치
        [HideInInspector]
        public float HP;
        [HideInInspector]
        public float MP;
        [HideInInspector]
        public float Mental;
    }
}
