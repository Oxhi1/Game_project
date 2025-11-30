using UnityEngine;

/// <summary>
/// Player character with 4 unique actions:
/// 1. Quick Attack - Low damage, low energy cost
/// 2. Heavy Attack - High damage, high energy cost
/// 3. Defend - Reduce incoming damage
/// 4. Poison Strike - Apply poison effect to opponent
/// </summary>
public class PlayerCharacter : Character
{
    [Header("Player Action Settings")]
    [SerializeField] private int quickAttackDamage = 10;
    [SerializeField] private int quickAttackEnergyCost = 10;
    
    [SerializeField] private int heavyAttackDamage = 25;
    [SerializeField] private int heavyAttackEnergyCost = 25;
    
    [SerializeField] private int defendEnergyCost = 15;
    
    [SerializeField] private int poisonStrikeDamage = 5;
    [SerializeField] private int poisonStrikeEnergyCost = 20;
    [SerializeField] private int poisonDuration = 3;
    [SerializeField] private int poisonDamagePerTurn = 5;

    // Action names for UI display
    private readonly string[] actionNames = new string[]
    {
        "Hızlı Saldırı",
        "Güçlü Saldırı",
        "Savunma",
        "Zehirli Darbe"
    };

    // Energy costs for each action
    private int[] actionEnergyCosts;

    protected override void Start()
    {
        base.Start();
        actionEnergyCosts = new int[]
        {
            quickAttackEnergyCost,
            heavyAttackEnergyCost,
            defendEnergyCost,
            poisonStrikeEnergyCost
        };
    }

    /// <summary>
    /// Perform one of the 4 player actions
    /// </summary>
    /// <param name="actionIndex">0: Quick Attack, 1: Heavy Attack, 2: Defend, 3: Poison Strike</param>
    /// <param name="target">The opponent character</param>
    public override void PerformAction(int actionIndex, Character target)
    {
        if (!CanPerformAction(actionIndex))
        {
            Debug.LogWarning("Cannot perform action: insufficient energy or stunned");
            return;
        }

        int energyCost = GetActionEnergyCost(actionIndex);
        if (!UseEnergy(energyCost))
        {
            return;
        }

        switch (actionIndex)
        {
            case 0: // Quick Attack
                QuickAttack(target);
                break;
            case 1: // Heavy Attack
                HeavyAttack(target);
                break;
            case 2: // Defend
                Defend();
                break;
            case 3: // Poison Strike
                PoisonStrike(target);
                break;
            default:
                Debug.LogError("Invalid action index: " + actionIndex);
                break;
        }
    }

    /// <summary>
    /// Quick Attack - Fast attack with low damage and low energy cost
    /// </summary>
    private void QuickAttack(Character target)
    {
        target.TakeDamage(quickAttackDamage);
        OnActionPerformed?.Invoke("Oyuncu hızlı saldırı yaptı! " + quickAttackDamage + " hasar verildi.");
        Debug.Log("Player performed Quick Attack for " + quickAttackDamage + " damage");
    }

    /// <summary>
    /// Heavy Attack - Powerful attack with high damage and high energy cost
    /// </summary>
    private void HeavyAttack(Character target)
    {
        target.TakeDamage(heavyAttackDamage);
        OnActionPerformed?.Invoke("Oyuncu güçlü saldırı yaptı! " + heavyAttackDamage + " hasar verildi.");
        Debug.Log("Player performed Heavy Attack for " + heavyAttackDamage + " damage");
    }

    /// <summary>
    /// Defend - Reduce incoming damage for next attack
    /// </summary>
    private void Defend()
    {
        SetDefending(true);
        OnActionPerformed?.Invoke("Oyuncu savunma pozisyonuna geçti!");
        Debug.Log("Player is now defending");
    }

    /// <summary>
    /// Poison Strike - Apply poison effect that deals damage over time
    /// </summary>
    private void PoisonStrike(Character target)
    {
        target.TakeDamage(poisonStrikeDamage);
        target.ApplyPoison(poisonDamagePerTurn, poisonDuration);
        OnActionPerformed?.Invoke("Oyuncu zehirli darbe attı! Rakip " + poisonDuration + " tur boyunca zehir hasarı alacak.");
        Debug.Log("Player performed Poison Strike - target poisoned for " + poisonDuration + " turns");
    }

    public override string GetActionName(int actionIndex)
    {
        if (actionIndex >= 0 && actionIndex < actionNames.Length)
        {
            return actionNames[actionIndex];
        }
        return "Bilinmeyen Aksiyon";
    }

    public override int GetActionEnergyCost(int actionIndex)
    {
        if (actionEnergyCosts == null || actionEnergyCosts.Length == 0)
        {
            // Initialize if not done in Start
            actionEnergyCosts = new int[]
            {
                quickAttackEnergyCost,
                heavyAttackEnergyCost,
                defendEnergyCost,
                poisonStrikeEnergyCost
            };
        }
        
        if (actionIndex >= 0 && actionIndex < actionEnergyCosts.Length)
        {
            return actionEnergyCosts[actionIndex];
        }
        return 0;
    }
}
