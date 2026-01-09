using UnityEngine;
using UnityEngine.AI;

public class PlayerAnimator : MonoBehaviour
{
    public Animator animator;
    public NavMeshAgent agent;
    
    [Header("Settings")]
    public float arrivalDistance = 0.5f;  // How close to destination before stopping animation
    
    // Animation parameter names (must match Animator Controller)
    private readonly string isMovingParam = "IsMoving";
    private readonly string speedParam = "Speed";

    void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
    }

    void Update()
    {
        if (animator == null || agent == null) return;

        float speed = agent.velocity.magnitude;
        
        // Simple check: does the agent have somewhere to go?
        bool isMoving = agent.hasPath && 
                        !agent.pathPending && 
                        agent.remainingDistance > arrivalDistance;
        
        // Send to Animator
        animator.SetBool(isMovingParam, isMoving);
        animator.SetFloat(speedParam, speed);
    }
}
