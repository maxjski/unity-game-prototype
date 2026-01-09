using UnityEngine;

// Attach this to your Combat Canvas.
// Have buttons call these methods.
public class CombatUIHelper : MonoBehaviour
{
    // For a "Flee" or "End Combat" button (for testing)
    public void EndCombat()
    {
        Enemy.EndCurrentCombat();
    }
}
