using UnityEngine;

// Persistent player stats across all fights
public static class PlayerStats
{
    public static float maxHP = 100f;
    public static float currentHP = 100f;
    
    // Call this to fully heal (e.g., at save points, new game)
    public static void FullHeal()
    {
        currentHP = maxHP;
    }

    // Call this when starting a new game
    public static void ResetStats()
    {
        maxHP = 100f;
        currentHP = maxHP;
    }

    // Take damage (returns true if still alive)
    public static bool TakeDamage(float damage)
    {
        currentHP -= damage;
        if (currentHP < 0) currentHP = 0;
        return currentHP > 0;
    }

    // Heal (won't exceed max)
    public static void Heal(float amount)
    {
        currentHP += amount;
        if (currentHP > maxHP) currentHP = maxHP;
    }
}
