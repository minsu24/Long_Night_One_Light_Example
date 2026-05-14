using UnityEngine;

public class Rope : MonoBehaviour
{
    GameObject player;
    PlayerController playerController;
    private  bool playerInRope;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(playerInRope && Input.GetKey(KeyCode.UpArrow))
        {
            GameManager.instance.isInputLocked = true;
            playerController.isClimbing = true;     
        }
        if (!playerController.isClimbing)
        {
            GameManager.instance.isInputLocked = false;
        }
    }

    // 줄의 중앙 X 위치를 반환하는 함수
    public float GetRopeCenterX() 
    {
        return GetComponent<Collider2D>().bounds.center.x;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRope = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRope = false;
            if (playerController.isClimbing)
            {
                GameManager.instance.isInputLocked = false;
                playerController.isClimbing = false;   
            }
        }
    }
}
