using UnityEngine;
using UnityEngine.Events;
using System;

/// <summary>
/// Base class for all characters (Player and Opponent)
/// Contains common properties and methods for character interactions
/// </summary>
public abstract class Character : MonoBehaviour
{
    [Header("Character Stats")]
    [SerializeField] protected int maxHealth = 100;
    [SerializeField] protected int currentHealth;
    [SerializeField] protected int maxEnergy = 100;
    [SerializeField] protected int currentEnergy;
    [SerializeField] protected int attackPower = 10;
    [SerializeField] protected int defensePower = 5;

    [Header("Status Effects")]
    [SerializeField] protected bool isDefending = false;
    [SerializeField] protected bool isPoisoned = false;
    [SerializeField] protected int poisonDamage = 5;
    [SerializeField] protected int poisonTurns = 0;
    [SerializeField] protected bool isStunned = false;
    [SerializeField] protected int stunTurns = 0;

    [Header("Events")]
    public UnityEvent<int, int> OnHealthChanged;
    public UnityEvent<int, int> OnEnergyChanged;
    public UnityEvent OnDeath;
    public UnityEvent<string> OnActionPerformed;

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public int MaxEnergy => maxEnergy;
    public int CurrentEnergy => currentEnergy;
    public bool IsDefending => isDefending;
    public bool IsStunned => isStunned;
    public bool IsPoisoned => isPoisoned;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        currentEnergy = maxEnergy;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
    }

    /// <summary>
    /// Take damage from an attack
    /// </summary>
    /// <param name="damage">Amount of damage to receive</param>
    public virtual void TakeDamage(int damage)
    {
        int actualDamage = damage;
        
        // Reduce damage if defending
        if (isDefending)
        {
            actualDamage = Mathf.Max(0, damage - defensePower);
            isDefending = false;
        }

        currentHealth = Mathf.Max(0, currentHealth - actualDamage);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
    }

    /// <summary>
    /// Heal the character
    /// </summary>
    /// <param name="amount">Amount of health to restore</param>
    public virtual void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    /// <summary>
    /// Restore energy
    /// </summary>
    /// <param name="amount">Amount of energy to restore</param>
    public virtual void RestoreEnergy(int amount)
    {
        currentEnergy = Mathf.Min(maxEnergy, currentEnergy + amount);
        OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
    }

    /// <summary>
    /// Use energy for an action
    /// </summary>
    /// <param name="amount">Amount of energy to use</param>
    /// <returns>True if energy was used successfully</returns>
    public virtual bool UseEnergy(int amount)
    {
        if (currentEnergy >= amount)
        {
            currentEnergy -= amount;
            OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Apply poison effect
    /// </summary>
    /// <param name="damage">Damage per turn</param>
    /// <param name="turns">Number of turns</param>
    public virtual void ApplyPoison(int damage, int turns)
    {
        isPoisoned = true;
        poisonDamage = damage;
        poisonTurns = turns;
    }

    /// <summary>
    /// Apply stun effect
    /// </summary>
    /// <param name="turns">Number of turns to stun</param>
    public virtual void ApplyStun(int turns)
    {
        isStunned = true;
        stunTurns = turns;
    }

    /// <summary>
    /// Process status effects at the start of turn
    /// </summary>
    public virtual void ProcessStatusEffects()
    {
        // Process poison
        if (isPoisoned && poisonTurns > 0)
        {
            TakeDamage(poisonDamage);
            poisonTurns--;
            if (poisonTurns <= 0)
            {
                isPoisoned = false;
            }
        }

        // Process stun
        if (isStunned && stunTurns > 0)
        {
            stunTurns--;
            if (stunTurns <= 0)
            {
                isStunned = false;
            }
        }
    }

    /// <summary>
    /// Set defense mode
    /// </summary>
    public virtual void SetDefending(bool defending)
    {
        isDefending = defending;
    }

    /// <summary>
    /// Abstract method for performing an action
    /// Each character must implement their own actions
    /// </summary>
    /// <param name="actionIndex">Index of the action to perform (0-3)</param>
    /// <param name="target">The target character</param>
    public abstract void PerformAction(int actionIndex, Character target);

    /// <summary>
    /// Get the name of an action
    /// </summary>
    /// <param name="actionIndex">Index of the action</param>
    /// <returns>Name of the action</returns>
    public abstract string GetActionName(int actionIndex);

    /// <summary>
    /// Get the energy cost of an action
    /// </summary>
    /// <param name="actionIndex">Index of the action</param>
    /// <returns>Energy cost</returns>
    public abstract int GetActionEnergyCost(int actionIndex);

    /// <summary>
    /// Check if an action can be performed
    /// </summary>
    /// <param name="actionIndex">Index of the action</param>
    /// <returns>True if the action can be performed</returns>
    public virtual bool CanPerformAction(int actionIndex)
    {
        if (isStunned) return false;
        return currentEnergy >= GetActionEnergyCost(actionIndex);
    }

    /// <summary>
    /// Reset character to initial state
    /// </summary>
    public virtual void ResetCharacter()
    {
        currentHealth = maxHealth;
        currentEnergy = maxEnergy;
        isDefending = false;
        isPoisoned = false;
        poisonTurns = 0;
        isStunned = false;
        stunTurns = 0;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
    }
}
