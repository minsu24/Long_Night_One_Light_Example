using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class DashSkill : SkillBase
{
    public float dashDistance = 50.0f; //대쉬 거리

    public GameObject player;
    public PlayerController playerController;
    private int playerLayer, EnemyLayer;
    SpriteRenderer spriteRenderer;

    Rigidbody2D rb;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        rb = playerController.GetComponent<Rigidbody2D>();
        spriteRenderer = player.GetComponent<SpriteRenderer>();
        EnemyLayer = LayerMask.NameToLayer("Enemy");
        playerLayer = LayerMask.NameToLayer("Player");
    }
    protected override void OnCast()
    {
        StartCoroutine("DashRoutine");
    }

    IEnumerator DashRoutine()
    {
        playerController.isDashing = true;
        float originalGravity = playerController.rb.gravityScale;
        playerController.rb.gravityScale = 0f;        
        playerController.isInvincible = true;
        Physics2D.IgnoreLayerCollision(EnemyLayer, playerLayer, true);
        Color c = spriteRenderer.color;
        c.a = 0.5f;
        spriteRenderer.color = c;
        float direction = Math.Sign(player.transform.localScale.x);
        rb.linearVelocity = new Vector2(direction * dashDistance, 0); 

        yield return new WaitForSeconds(0.5f);
        playerController.rb.gravityScale = originalGravity;
        Physics2D.IgnoreLayerCollision(EnemyLayer, playerLayer, false);
        c.a = 1.0f;
        spriteRenderer.color = c;
        playerController.isInvincible = false;

        playerController.isDashing = false;
        rb.linearVelocity = Vector2.zero;
        
    }
}
