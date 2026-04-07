using UnityEngine;

public class LightFragment : MonoBehaviour
{
    [SerializeField] private float mentalRecovery = 10f;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            PlayerController playerController = collision.GetComponent<PlayerController>();
            playerController.Mental += mentalRecovery;
            Destroy(gameObject);
        }
    }
}