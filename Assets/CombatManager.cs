using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CombatManager : MonoBehaviour
{
    [Header("Stats")]
    public float enemyMaxHP = 50f;
    public float playerAttackDamage = 15f;
    public float enemyAttackDamage = 10f;

    [Header("UI - HP Bars")]
    public Image playerHPBar;
    public Image enemyHPBar;
    
    // Enemy HP is per-fight, Player HP persists via PlayerStats

    [Header("UI - Player Sprites")]
    public GameObject playerNormalSprite;  // Normal player image
    public GameObject playerDodgeSprite;   // Dodging player image

    [Header("UI - Attack Indicator")]
    public Image attackIndicator;          // Fill image for timing
    public float attackChargeTime = 1.5f;  // Time for indicator to fill

    [Header("UI - Optional Text")]
    public TMPro.TextMeshProUGUI combatLogText;

    private float enemyCurrentHP;
    private bool playerTurn = true;
    private bool combatEnded = false;

    // Dodge system
    private bool isEnemyAttacking = false;
    private float attackTimer = 0f;
    private bool playerDodged = false;
    private bool hasAttemptedDodge = false; // Only one dodge attempt per attack
    private float dodgeWindowStart = 0.9f;  // Dodge window starts at 90% fill
    private float dodgeWindowEnd = 1.0f;    // Dodge window ends at 100%
    private float dodgeDuration = 0.2f;     // How long dodge animation lasts
    private float dodgeTimer = 0f;
    private bool isDodging = false;

    void OnEnable()
    {
        // Reset combat when canvas is shown
        StartCombat();
    }

    void StartCombat()
    {
        // Player HP persists from PlayerStats, only reset enemy HP
        enemyCurrentHP = enemyMaxHP;
        playerTurn = true;
        combatEnded = false;
        isEnemyAttacking = false;
        playerDodged = false;
        isDodging = false;
        
        // Reset sprites
        if (playerNormalSprite != null) playerNormalSprite.SetActive(true);
        if (playerDodgeSprite != null) playerDodgeSprite.SetActive(false);
        if (attackIndicator != null) attackIndicator.fillAmount = 0f;
        
        UpdateHPBars();
        Log($"Combat started! HP: {PlayerStats.currentHP}/{PlayerStats.maxHP}");
    }

    void Update()
    {
        // Handle dodge input during enemy attack (only one attempt allowed)
        if (isEnemyAttacking && !hasAttemptedDodge && Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            TryDodge();
        }

        // Update attack indicator
        if (isEnemyAttacking)
        {
            UpdateAttackIndicator();
        }

        // Handle dodge animation timer
        if (isDodging)
        {
            dodgeTimer -= Time.deltaTime;
            if (dodgeTimer <= 0)
            {
                EndDodgeAnimation();
            }
        }
    }

    void TryDodge()
    {
        hasAttemptedDodge = true; // Only one attempt per attack
        float fillPercent = attackTimer / attackChargeTime;
        
        // Check if within dodge window (90% - 100%)
        if (fillPercent >= dodgeWindowStart && fillPercent <= dodgeWindowEnd)
        {
            playerDodged = true;
            Log("Perfect dodge!");
        }
        else
        {
            Log("Missed timing!");
        }

        // Show dodge animation regardless
        StartDodgeAnimation();
    }

    void StartDodgeAnimation()
    {
        isDodging = true;
        dodgeTimer = dodgeDuration;
        
        if (playerNormalSprite != null) playerNormalSprite.SetActive(false);
        if (playerDodgeSprite != null) playerDodgeSprite.SetActive(true);
    }

    void EndDodgeAnimation()
    {
        isDodging = false;
        
        if (playerNormalSprite != null) playerNormalSprite.SetActive(true);
        if (playerDodgeSprite != null) playerDodgeSprite.SetActive(false);
    }

    void UpdateAttackIndicator()
    {
        attackTimer += Time.deltaTime;
        
        // Ease-in curve: starts slow, gets faster
        float t = attackTimer / attackChargeTime;
        float easedT = t * t; // Quadratic ease-in
        
        if (attackIndicator != null)
        {
            attackIndicator.fillAmount = easedT;
        }

        // Attack lands when fully charged
        if (t >= 1f)
        {
            EnemyAttackLands();
        }
    }

    // Called by the Attack button
    public void PlayerAttack()
    {
        if (!playerTurn || combatEnded) return;

        // Player attacks enemy
        enemyCurrentHP -= playerAttackDamage;
        Log($"You deal {playerAttackDamage} damage!");
        
        UpdateHPBars();

        // Check if enemy died
        if (enemyCurrentHP <= 0)
        {
            enemyCurrentHP = 0;
            UpdateHPBars();
            Victory();
            return;
        }

        // Enemy's turn
        playerTurn = false;
        Invoke(nameof(EnemyTurn), 0.5f); // Small delay for effect
    }

    void EnemyTurn()
    {
        if (combatEnded) return;

        // Start the attack charge-up
        isEnemyAttacking = true;
        attackTimer = 0f;
        playerDodged = false;
        hasAttemptedDodge = false; // Reset for new attack
        
        if (attackIndicator != null) attackIndicator.fillAmount = 0f;
        
        Log("Enemy is attacking! Press SPACE to dodge!");
    }

    void EnemyAttackLands()
    {
        isEnemyAttacking = false;
        
        if (attackIndicator != null) attackIndicator.fillAmount = 0f;

        // Apply damage based on dodge
        if (playerDodged)
        {
            Log("You dodged the attack!");
            // No damage taken
        }
        else
        {
            PlayerStats.TakeDamage(enemyAttackDamage);
            Log($"Enemy deals {enemyAttackDamage} damage!");
        }
        
        UpdateHPBars();

        // Check if player died
        if (PlayerStats.currentHP <= 0)
        {
            UpdateHPBars();
            Defeat();
            return;
        }

        // Back to player's turn after short delay
        Invoke(nameof(StartPlayerTurn), 0.5f);
    }

    void StartPlayerTurn()
    {
        playerTurn = true;
        Log("Your turn!");
    }

    void UpdateHPBars()
    {
        if (playerHPBar != null)
        {
            playerHPBar.fillAmount = PlayerStats.currentHP / PlayerStats.maxHP;
            Debug.Log($"Player HP: {PlayerStats.currentHP}/{PlayerStats.maxHP} = {playerHPBar.fillAmount}");
        }
        else
        {
            Debug.LogWarning("Player HP Bar is not assigned!");
        }
        
        if (enemyHPBar != null)
        {
            enemyHPBar.fillAmount = enemyCurrentHP / enemyMaxHP;
            Debug.Log($"Enemy HP: {enemyCurrentHP}/{enemyMaxHP} = {enemyHPBar.fillAmount}");
        }
        else
        {
            Debug.LogWarning("Enemy HP Bar is not assigned!");
        }
    }

    void Victory()
    {
        combatEnded = true;
        Log("Victory! Enemy defeated!");
        
        // End combat after a short delay
        Invoke(nameof(EndCombatVictory), 1.5f);
    }

    void Defeat()
    {
        combatEnded = true;
        Log("Defeat! You died!");
        
        // For now, just end combat - later you could show game over
        Invoke(nameof(EndCombatDefeat), 1.5f);
    }

    void EndCombatVictory()
    {
        Enemy.EndCurrentCombat(); // Destroys enemy
    }

    void EndCombatDefeat()
    {
        // For now, same as victory - later you could handle game over
        Enemy.EndCurrentCombat();
    }

    void Log(string message)
    {
        Debug.Log(message);
        if (combatLogText != null)
        {
            combatLogText.text = message;
        }
    }
}
