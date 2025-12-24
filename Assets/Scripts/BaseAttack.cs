using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class BaseAttack : MonoBehaviour
{
    [SerializeField]
    private GameObject BaseAttackPrefab;

    public float cooltime;
    private float curtime;


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

            if (Input.GetKey(KeyCode.A))
            {
                Debug.Log("공격");
                GameObject fire = Instantiate(BaseAttackPrefab, transform.position, Quaternion.identity);
                Fire fireScript = fire.GetComponent<Fire>();
                if(fireScript != null)
                {
                    Vector3 dir = transform.root.localScale.x < 0 ? Vector3.left : Vector3.right;
                    fireScript.SetDirection(dir);
                    if(dir == Vector3.left)
                    {
                        fire.GetComponent<SpriteRenderer>().flipX = true;
                    }
                }
            }
            curtime = cooltime;
        }
        curtime -= Time.deltaTime;
        
    }
}
