using UnityEngine;

/// <summary>
/// Opponent character with 4 unique actions:
/// 1. Slash - Basic attack
/// 2. Power Slam - Heavy attack with stun chance
/// 3. Guard - Defense stance
/// 4. Life Drain - Damage and heal
/// 
/// This character uses rule-based AI (no machine learning)
/// The AI logic can be extended in the future
/// </summary>
public class OpponentCharacter : Character
{
    [Header("Opponent Action Settings")]
    [SerializeField] private int slashDamage = 12;
    [SerializeField] private int slashEnergyCost = 10;
    
    [SerializeField] private int powerSlamDamage = 20;
    [SerializeField] private int powerSlamEnergyCost = 30;
    [SerializeField] private int stunDuration = 1;
    
    [SerializeField] private int guardEnergyCost = 15;
    
    [SerializeField] private int lifeDrainDamage = 15;
    [SerializeField] private int lifeDrainHeal = 10;
    [SerializeField] private int lifeDrainEnergyCost = 25;

    // Action names for UI display
    private readonly string[] actionNames = new string[]
    {
        "Kesici Saldırı",
        "Güçlü Darbe",
        "Koruma",
        "Can Çalma"
    };

    // Energy costs for each action
    private int[] actionEnergyCosts;

    // Reference to AI controller (can be extended in the future)
    private OpponentAI aiController;

    /// <summary>
    /// Initialize the energy costs array
    /// </summary>
    private void InitializeEnergyCosts()
    {
        actionEnergyCosts = new int[]
        {
            slashEnergyCost,
            powerSlamEnergyCost,
            guardEnergyCost,
            lifeDrainEnergyCost
        };
    }

    protected override void Start()
    {
        base.Start();
        InitializeEnergyCosts();

        // Initialize AI controller
        aiController = GetComponent<OpponentAI>();
        if (aiController == null)
        {
            aiController = gameObject.AddComponent<OpponentAI>();
        }
    }

    /// <summary>
    /// Perform one of the 4 opponent actions
    /// </summary>
    /// <param name="actionIndex">0: Slash, 1: Power Slam, 2: Guard, 3: Life Drain</param>
    /// <param name="target">The player character</param>
    public override void PerformAction(int actionIndex, Character target)
    {
        if (!CanPerformAction(actionIndex))
        {
            Debug.LogWarning("Opponent cannot perform action: insufficient energy or stunned");
            return;
        }

        int energyCost = GetActionEnergyCost(actionIndex);
        if (!UseEnergy(energyCost))
        {
            return;
        }

        switch (actionIndex)
        {
            case 0: // Slash
                Slash(target);
                break;
            case 1: // Power Slam
                PowerSlam(target);
                break;
            case 2: // Guard
                Guard();
                break;
            case 3: // Life Drain
                LifeDrain(target);
                break;
            default:
                Debug.LogError("Invalid action index: " + actionIndex);
                break;
        }
    }

    /// <summary>
    /// Slash - Basic attack with moderate damage
    /// </summary>
    private void Slash(Character target)
    {
        target.TakeDamage(slashDamage);
        OnActionPerformed?.Invoke("Rakip kesici saldırı yaptı! " + slashDamage + " hasar verildi.");
        Debug.Log("Opponent performed Slash for " + slashDamage + " damage");
    }

    /// <summary>
    /// Power Slam - Heavy attack that also stuns the target
    /// </summary>
    private void PowerSlam(Character target)
    {
        target.TakeDamage(powerSlamDamage);
        target.ApplyStun(stunDuration);
        OnActionPerformed?.Invoke("Rakip güçlü darbe attı! " + powerSlamDamage + " hasar verildi ve oyuncu " + stunDuration + " tur sersemledi.");
        Debug.Log("Opponent performed Power Slam for " + powerSlamDamage + " damage and stunned target");
    }

    /// <summary>
    /// Guard - Enter defensive stance
    /// </summary>
    private void Guard()
    {
        SetDefending(true);
        OnActionPerformed?.Invoke("Rakip savunma pozisyonuna geçti!");
        Debug.Log("Opponent is now guarding");
    }

    /// <summary>
    /// Life Drain - Deal damage and heal self
    /// </summary>
    private void LifeDrain(Character target)
    {
        target.TakeDamage(lifeDrainDamage);
        Heal(lifeDrainHeal);
        OnActionPerformed?.Invoke("Rakip can çaldı! " + lifeDrainDamage + " hasar verdi ve " + lifeDrainHeal + " can yeniledi.");
        Debug.Log("Opponent performed Life Drain - dealt " + lifeDrainDamage + " damage and healed " + lifeDrainHeal);
    }

    /// <summary>
    /// Get AI decision for next action
    /// This method uses rule-based AI logic
    /// </summary>
    /// <param name="player">Reference to player for decision making</param>
    /// <returns>Action index to perform</returns>
    public int GetAIDecision(Character player)
    {
        if (aiController != null)
        {
            return aiController.DecideAction(this, player);
        }
        
        // Fallback: random action if no AI controller
        return Random.Range(0, 4);
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
        // Ensure initialization if accessed before Start
        if (actionEnergyCosts == null)
        {
            InitializeEnergyCosts();
        }
        
        if (actionIndex >= 0 && actionIndex < actionEnergyCosts.Length)
        {
            return actionEnergyCosts[actionIndex];
        }
        return 0;
    }
}
