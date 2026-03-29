using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerAttackSystem : MonoBehaviour
{
    public static PlayerAttackSystem instance;
    [SerializeField]
    private GameObject BaseAttackPrefab; // 발사체 프리팹
    [SerializeField] private SkillBase sSkill;
    [SerializeField] private SkillBase dashSkill;

    private GameObject player;
    private GameObject fireSpirit;
    private PlayerController playerController;

    public float cooltime; // 공격 쿨타임
    public float chargeCooltime; // 차지 공격 쿨타임
    private float curtime; // 쿨타임 계산을 위한 변수
    private float chargeCurtime; //차지 공격 쿨타임 계산을 위한 변수

    public Sprite fireIcon;

    public Sprite speedIcon;
    private Sprite icon;
    public bool baseAttacking = false;
    public bool chargeAttaking = false;
    public bool S_Skilling = false;

    void Awake()
    {
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        fireSpirit = GameObject.FindGameObjectWithTag("Friend");
    }

    void Update()
    {
        if(curtime > 0) curtime -= Time.deltaTime;
        if(chargeCurtime > 0) chargeCurtime -=Time.deltaTime; // 쿨타임 동작을 위해 추가함 3/28
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if(context.action.name == "Attack"){
            if (context.canceled)
            {
                if(curtime <= 0 && !chargeAttaking) // 차지 공격 중이 아니고 쿨타임이 돌았다면 
                {
                    if(context.duration < 1.0f) // 공격 키를 1초 미만으로 눌렀을 때
                    {   
                        baseAttacking = true;
                        Debug.Log("일반 공격");
                        GameObject fire = Instantiate(BaseAttackPrefab, transform.position, Quaternion.identity); // 발사체 생성
                        Fire fireScript = fire.GetComponent<Fire>(); 
                        if(fireScript != null)
                        {
                            fireScript.damage = playerController.FinalDamage; // 발사체 데미지 설정
                            fireScript.isCharge = false;
                            float dir = transform.root.localScale.x; // 발사체 방향 전달 
                            fireScript.SetDirection(dir);
                            if(dir == -1)
                            {
                                fire.GetComponent<SpriteRenderer>().flipX = true;
                            }
                        }
                        curtime = cooltime;
                    }                
                }

            }
            if (context.performed) //  공격 키를 1초 이상으로 눌렀을 때
            {
                if(chargeCurtime <= 0 && !baseAttacking) // 기본 공격중이 아니고 차지 공격 쿨타임이 돌았다면  
                {
                    chargeAttaking = true;
                    fireSpirit.SetActive(false);
                    Debug.Log("차지 공격");
                    GameObject fire = Instantiate(BaseAttackPrefab, transform.position, Quaternion.identity); // 발사체 생성
                    Fire fireScript = fire.GetComponent<Fire>(); 
                    if(fireScript != null)
                    {
                        fireScript.Setup(fireSpirit); // 차지 공격이 Setup()을 먼저 호출해서 순서를 마추기 위해 수정 3/28
                        fireScript.damage = playerController.FinalDamage;
                        fireScript.isCharge = true;
                        float dir = transform.root.localScale.x; // 발사체 방향 전달 
                        fireScript.SetDirection(dir);
                        if(dir == -1)
                        {
                            fire.GetComponent<SpriteRenderer>().flipX = true;
                        }
                    }
                    curtime = cooltime;
                }

            }
        }
        else if(context.action.name == "S_Skill") //화염 휩쓸기
        {
            if(context.started){
                if(playerController.MP < 50 || !sSkill.IsReady)
                {
                    Debug.Log("S스킬 사용불가");
                }
                else
                {
                    Debug.Log("S스킬 사용가능");
                    playerController.MP -= 50;
                    icon = fireIcon;
                    BuffManager.instance.CreateBuff("Atk", 300f, 5f, icon); // 버프 아이콘 생성에 필요한 데이터 전달
                    sSkill.ExecuteSkill();
                }
            }
        }
        else if(context.action.name == "Dash") // 대쉬 
        {
            if (context.started)
            {
                if(playerController.Stamina < 20 || !dashSkill.IsReady)
                {
                    Debug.Log("대쉬 스킬 사용불가");
                }
                else
                {
                    Debug.Log("대쉬스킬 사용가능");
                    playerController.Stamina -= 20;
                    dashSkill.ExecuteSkill();
                }
            }
        }
        else if(context.action.name == "Speed_Up") // 이속 증가 스킬
        {
            if (context.started)
            {
                icon = speedIcon;
                BuffManager.instance.CreateBuff("SPEED", 150f, 5f, icon);
            }
        }
    }
    
   
}
