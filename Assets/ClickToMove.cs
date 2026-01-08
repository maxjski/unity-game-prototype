using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class ClickToMove : MonoBehaviour
{
    public NavMeshAgent agent;
    public float interactionDistance = 0.5f; // How close before showing UI

    private InteractableItem targetItem;
    private bool isInteracting = false;

    void Start()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
    }

    void Update()
    {
        if (agent == null) return;
        if (Mouse.current == null) return;

        // Handle clicking
        if (Mouse.current.leftButton.wasPressedThisFrame && !isInteracting)
        {
            Camera cam = Camera.main;
            if (cam == null)
            {
                Debug.LogError("ClickToMove: No main camera found! Make sure your camera is tagged 'MainCamera'.");
                return;
            }

            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = cam.ScreenPointToRay(mousePos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                InteractableItem item = hit.collider.GetComponent<InteractableItem>();

                if (item != null)
                {
                    // We clicked an item - walk to its interaction point
                    targetItem = item;
                    
                    if (item.interactionPoint != null)
                    {
                        agent.SetDestination(item.interactionPoint.position);
                    }
                    else
                    {
                        // No interaction point set, walk to the item itself
                        agent.SetDestination(hit.point);
                    }
                }
                else
                {
                    // We clicked the floor - clear target and move there
                    targetItem = null;
                    agent.SetDestination(hit.point);
                }
            }
        }

        // Check if we've arrived at an item's interaction point
        if (targetItem != null && !agent.pathPending && agent.remainingDistance <= interactionDistance && !isInteracting)
        {
            targetItem.ShowUI(this); // Pass reference so item can call us back
            targetItem = null;
            isInteracting = true;
        }
    }

    // Called by InteractableItem when the menu is closed
    public void ResumeMovement()
    {
        isInteracting = false;
    }
}
