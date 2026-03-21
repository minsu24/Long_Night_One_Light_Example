using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class DashSkill : SkillBase
{
    public float dashDistance = 50.0f; //대쉬 거리

    public GameObject player;
    public PlayerController playerController;

    Rigidbody2D rb;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        rb = playerController.GetComponent<Rigidbody2D>();
    }
    protected override void OnCast()
    {
        StartCoroutine("DashRoutine");
    }

    IEnumerator DashRoutine()
    {
        playerController.isDashing = true;

        float direction = Math.Sign(player.transform.localScale.x);
        rb.linearVelocity = new Vector2(direction * dashDistance, 0); 

        yield return new WaitForSeconds(0.5f);

        playerController.isDashing = false;
        rb.linearVelocity = Vector2.zero;
    }
}
