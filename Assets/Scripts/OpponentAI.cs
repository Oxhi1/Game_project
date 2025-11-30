using UnityEngine;

/// <summary>
/// Rule-based AI for the opponent character
/// This class contains the decision logic for the opponent's actions
/// 
/// DESIGN NOTE: This AI uses simple rule-based logic (no machine learning)
/// Future extensions could include:
/// - Machine Learning based AI
/// - Difficulty levels
/// - Adaptive behavior based on player patterns
/// </summary>
public class OpponentAI : MonoBehaviour
{
    [Header("AI Behavior Settings")]
    [Tooltip("Health threshold percentage below which AI prioritizes healing")]
    [SerializeField] private float lowHealthThreshold = 0.3f;
    
    [Tooltip("Health threshold percentage below which AI considers defending")]
    [SerializeField] private float defendHealthThreshold = 0.5f;
    
    [Tooltip("Energy threshold below which AI uses basic attacks")]
    [SerializeField] private int lowEnergyThreshold = 30;
    
    [Tooltip("Random factor for unpredictability (0-1)")]
    [Range(0f, 1f)]
    [SerializeField] private float randomFactor = 0.2f;

    /// <summary>
    /// Decide which action the opponent should take based on current game state
    /// Uses rule-based logic (no AI/ML required)
    /// </summary>
    /// <param name="opponent">The opponent character making the decision</param>
    /// <param name="player">The player character (target)</param>
    /// <returns>Action index (0-3)</returns>
    public int DecideAction(OpponentCharacter opponent, Character player)
    {
        // Add randomness to make AI less predictable
        if (Random.value < randomFactor)
        {
            return GetRandomValidAction(opponent);
        }

        float opponentHealthPercent = (float)opponent.CurrentHealth / opponent.MaxHealth;
        float playerHealthPercent = (float)player.CurrentHealth / player.MaxHealth;
        int currentEnergy = opponent.CurrentEnergy;

        // Rule 1: If health is very low and can heal, use Life Drain
        if (opponentHealthPercent < lowHealthThreshold && currentEnergy >= 25)
        {
            return 3; // Life Drain
        }

        // Rule 2: If health is moderately low, consider defending
        if (opponentHealthPercent < defendHealthThreshold && currentEnergy >= 15)
        {
            // 50% chance to defend when health is low
            if (Random.value < 0.5f)
            {
                return 2; // Guard
            }
        }

        // Rule 3: If player health is low, go for the kill with Power Slam
        if (playerHealthPercent < 0.25f && currentEnergy >= 30)
        {
            return 1; // Power Slam
        }

        // Rule 4: If energy is low, use basic attack
        if (currentEnergy < lowEnergyThreshold)
        {
            return 0; // Slash (lowest energy cost)
        }

        // Rule 5: If player is not defending and we have energy, use strong attack
        if (!player.IsDefending && currentEnergy >= 30)
        {
            // 60% chance for Power Slam, 40% for Slash
            if (Random.value < 0.6f)
            {
                return 1; // Power Slam
            }
        }

        // Rule 6: Balanced decision based on energy
        if (currentEnergy >= 25)
        {
            // Choose between Life Drain and Power Slam based on health
            if (opponentHealthPercent < 0.7f)
            {
                return 3; // Life Drain (self-heal)
            }
            else
            {
                return 1; // Power Slam (damage)
            }
        }

        // Default: Basic attack
        return 0; // Slash
    }

    /// <summary>
    /// Get a random valid action that the opponent can perform
    /// </summary>
    private int GetRandomValidAction(OpponentCharacter opponent)
    {
        int attempts = 10;
        while (attempts > 0)
        {
            int action = Random.Range(0, 4);
            if (opponent.CanPerformAction(action))
            {
                return action;
            }
            attempts--;
        }
        
        // Fallback to basic attack
        return 0;
    }

    /// <summary>
    /// Set AI difficulty by adjusting behavior parameters
    /// Can be used for future difficulty settings
    /// </summary>
    /// <param name="difficulty">0: Easy, 1: Normal, 2: Hard</param>
    public void SetDifficulty(int difficulty)
    {
        switch (difficulty)
        {
            case 0: // Easy - More random, less optimal
                randomFactor = 0.4f;
                lowHealthThreshold = 0.2f;
                break;
            case 1: // Normal
                randomFactor = 0.2f;
                lowHealthThreshold = 0.3f;
                break;
            case 2: // Hard - Less random, more strategic
                randomFactor = 0.1f;
                lowHealthThreshold = 0.4f;
                break;
        }
    }
}
