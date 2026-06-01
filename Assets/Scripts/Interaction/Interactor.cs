using System;
using UnityEngine;
using UnityEngine.Audio;

interface IInteractable
{
    public void Interact();
}
public class Interactor : MonoBehaviour
{
    public float InteractRange;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Vector2 dir = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, InteractRange, LayerMask.GetMask("Interact"));
            if(hit.collider != null)
            {
                if(hit.collider.TryGetComponent(out IInteractable interactObj))
                {
                    Debug.Log("오브젝트 상호");
                    interactObj.Interact();
                }
            }
            else
            {
                Debug.Log("상호작용 가능한 물체 없음");
            }
            
        }
    }
}
