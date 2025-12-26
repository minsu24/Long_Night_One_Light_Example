using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class BaseAttack : MonoBehaviour
{
    [SerializeField]
    private GameObject BaseAttackPrefab; // 발사체 프리팹

    public float cooltime; // 공격 쿨타임
    private float curtime; // 쿨타임 계산을 위한 변수
    private float presstime; // A키를 누른 시간 계산을 위한 변수


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(curtime <= 0)
        {
            Debug.Log("공격가능");

            if (Input.GetKeyDown(KeyCode.A)) // A키를 누르면 공격
            {
                presstime = Time.time;
                Debug.Log("공격");
                GameObject fire = Instantiate(BaseAttackPrefab, transform.position, Quaternion.identity); // 발사체 생성
                Fire fireScript = fire.GetComponent<Fire>(); 
                if(fireScript != null)
                {
                    float dir = transform.root.localScale.x; // 발사체 방향 지정
                    fireScript.SetDirection(dir);
                    if(dir == -1)
                    {
                        fire.GetComponent<SpriteRenderer>().flipX = true;
                    }
                }
                curtime = cooltime;
            }
        }
        else
        {
        curtime -= Time.deltaTime;
        }        
    }
}
