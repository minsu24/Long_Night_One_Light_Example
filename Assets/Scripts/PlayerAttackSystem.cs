using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject BaseAttackPrefab; // 발사체 프리팹

    public float cooltime; // 공격 쿨타임
    public float chargeCooltime; // 차지 공격 쿨타임
    private float curtime; // 쿨타임 계산을 위한 변수
    private float chargeCurtime; //차지 공격 쿨타임 계산을 위한 변수

    void Update()
    {
        if(curtime > 0) curtime -= Time.deltaTime;
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            if(curtime <= 0)
            {
                if(context.duration < 1.0f)
                {
                    Debug.Log("일반 공격");
                    GameObject fire = Instantiate(BaseAttackPrefab, transform.position, Quaternion.identity); // 발사체 생성
                    Fire fireScript = fire.GetComponent<Fire>(); 
                    if(fireScript != null)
                    {
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
        if (context.performed)
        {
            if(chargeCurtime <= 0)
            {
                Debug.Log("차지 공격");
                GameObject fire = Instantiate(BaseAttackPrefab, transform.position, Quaternion.identity); // 발사체 생성
                Fire fireScript = fire.GetComponent<Fire>(); 
                if(fireScript != null)
                {
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
}
