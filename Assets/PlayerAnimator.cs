using UnityEngine;
using UnityEngine.AI;

public class PlayerAnimator : MonoBehaviour
{
    public Animator animator;
    public NavMeshAgent agent;
    
    // Animation parameter names (must match Animator Controller)
    private readonly string speedParam = "Speed";

    void Start()
    {
        // Auto-find components if not assigned
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

        // Get the agent's current speed
        float speed = agent.velocity.magnitude;
        
        // Send speed to Animator
        animator.SetFloat(speedParam, speed);
    }
}
