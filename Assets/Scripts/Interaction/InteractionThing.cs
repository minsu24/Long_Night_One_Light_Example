using UnityEngine;

public class InteractionThing : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("상호작용 완료");
    }
}
