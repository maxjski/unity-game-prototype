using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class ClickToMove : MonoBehaviour
{
    public NavMeshAgent agent;
    public float interactionDistance = 0.5f; // How close before showing UI
    public GameObject clickIndicatorPrefab; // Drag the click indicator prefab here
    public LayerMask clickableLayers; // Set to "Ground" and "Interactable" layers

    private InteractableItem targetItem;
    private bool isInteracting = false;

    void Start()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
        
        // Instant acceleration (no sliding)
        if (agent != null)
        {
            agent.acceleration = 999f;
            agent.angularSpeed = 999f; // Fast turning too
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

            // Only detect objects on the specified layers
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, clickableLayers))
            {
                Debug.Log($"HIT: '{hit.collider.gameObject.name}' | Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)} | Position: {hit.point} | Collider type: {hit.collider.GetType().Name}");
                
                // Draw a debug line at hit point (visible in Scene view for 2 seconds)
                Debug.DrawRay(hit.point, Vector3.up * 2f, Color.red, 2f);
                Debug.DrawRay(hit.point, hit.normal * 1f, Color.green, 2f);
                
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
                    
                    // Spawn click indicator
                    SpawnClickIndicator(hit.point, hit.normal);
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

    // Called by Enemy when combat starts
    public void EnterCombat()
    {
        isInteracting = true;
        if (agent != null)
        {
            agent.ResetPath(); // Stop moving
        }
    }

    // Called by Enemy when combat ends
    public void ExitCombat()
    {
        isInteracting = false;
    }

    void SpawnClickIndicator(Vector3 position, Vector3 normal)
    {
        if (clickIndicatorPrefab == null) return;
        
        // Spawn slightly above ground to avoid z-fighting
        Vector3 spawnPos = position + normal * 0.01f;
        
        // Rotate to lie flat on the surface
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, normal);
        
        Instantiate(clickIndicatorPrefab, spawnPos, rotation);
    }
}
