using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MON_ADH_001 : EnemyController
{
    Vector3 lasttransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void MonsterAbility()
    {
        StartCoroutine(StopPlayer());
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if(other.transform.tag == "Player")
        {
            Debug.Log("잡기 발동");
            MonsterAbility();
        }
    }

    IEnumerator StopPlayer()
    {
        lasttransform = player.transform.position;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
        player.transform.position = gameObject.transform.position;
        GameManager.instance.isInputLocked = true;
        playerController.animator.SetBool("isMoving", false);
        yield return new WaitForSeconds(1f);
        GameManager.instance.isInputLocked = false;
        playerController.animator.SetBool("isMoving", true);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
        if(playerController.h > 0)
        {
            player.transform.position = lasttransform + new Vector3(-1, 0, 0);
        }
        else
        {
            player.transform.position = lasttransform + new Vector3(1, 0, 0);  
        }
        
    }
    
}
