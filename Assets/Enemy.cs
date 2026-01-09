using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float detectionRange = 3f;
    public GameObject combatCanvas; // Drag your combat canvas here
    
    private Transform player;
    private ClickToMove playerMovement;
    private bool inCombat = false;
    
    // Track currently active combat (so canvas buttons can end it)
    private static Enemy currentCombat;

    void Start()
    {
        // Find the player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerMovement = playerObj.GetComponent<ClickToMove>();
        }

        // Hide combat canvas at start
        if (combatCanvas != null)
        {
            combatCanvas.SetActive(false);
        }
    }

    void Update()
    {
        if (player == null || inCombat) return;

        // Check distance to player
        float distance = Vector3.Distance(transform.position, player.position);
        
        if (distance <= detectionRange)
        {
            StartCombat();
        }
    }

    void StartCombat()
    {
        inCombat = true;
        currentCombat = this;
        
        // Stop player movement
        if (playerMovement != null)
        {
            playerMovement.EnterCombat();
        }

        // Show combat canvas
        if (combatCanvas != null)
        {
            combatCanvas.SetActive(true);
        }

        Debug.Log($"Combat started with {gameObject.name}!");
    }

    public void EndCombat(bool destroyEnemy = true)
    {
        inCombat = false;
        currentCombat = null;
        
        // Resume player movement
        if (playerMovement != null)
        {
            playerMovement.ExitCombat();
        }

        // Hide combat canvas
        if (combatCanvas != null)
        {
            combatCanvas.SetActive(false);
        }

        Debug.Log("Combat ended!");

        // Destroy the enemy (for now - later you can check HP, etc.)
        if (destroyEnemy)
        {
            Destroy(gameObject);
        }
    }

    // Static method for UI button to call
    public static void EndCurrentCombat()
    {
        if (currentCombat != null)
        {
            currentCombat.EndCombat();
        }
    }

    // Visualize detection range in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
