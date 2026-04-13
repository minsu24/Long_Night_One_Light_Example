using UnityEngine;

public class BonFire : MonoBehaviour, IInteractable
{
    [SerializeField]
    private float recoverAmount = 10f;
    PlayerController playerController;
    private void Awake() 
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }
    public void Interact()
    {
        playerController.Mental = Mathf.Min(playerController.Mental + recoverAmount, playerController.maxMental);
    }

}
