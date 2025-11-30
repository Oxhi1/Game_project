using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// Main game manager that controls the game flow
/// Handles turn-based combat between player and opponent
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Character References")]
    [SerializeField] private PlayerCharacter player;
    [SerializeField] private OpponentCharacter opponent;

    [Header("Game State")]
    [SerializeField] private bool isPlayerTurn = true;
    [SerializeField] private bool isGameOver = false;
    [SerializeField] private int turnCount = 0;

    [Header("Energy Regeneration")]
    [SerializeField] private int energyRegenPerTurn = 15;

    [Header("Events")]
    public UnityEvent OnPlayerTurnStart;
    public UnityEvent OnOpponentTurnStart;
    public UnityEvent<string> OnGameOver; // Winner name
    public UnityEvent<string> OnBattleLog;
    public UnityEvent<int> OnTurnChanged;

    public bool IsPlayerTurn => isPlayerTurn;
    public bool IsGameOver => isGameOver;
    public int TurnCount => turnCount;
    public PlayerCharacter Player => player;
    public OpponentCharacter Opponent => opponent;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        InitializeGame();
    }

    /// <summary>
    /// Initialize or reset the game
    /// </summary>
    public void InitializeGame()
    {
        isGameOver = false;
        isPlayerTurn = true;
        turnCount = 1;

        if (player != null)
        {
            player.ResetCharacter();
            player.OnDeath.AddListener(OnPlayerDeath);
        }

        if (opponent != null)
        {
            opponent.ResetCharacter();
            opponent.OnDeath.AddListener(OnOpponentDeath);
        }

        OnTurnChanged?.Invoke(turnCount);
        OnPlayerTurnStart?.Invoke();
        LogBattle("Oyun başladı! Tur " + turnCount);
    }

    /// <summary>
    /// Called when player performs an action
    /// </summary>
    /// <param name="actionIndex">Index of the action (0-3)</param>
    public void PlayerAction(int actionIndex)
    {
        if (isGameOver || !isPlayerTurn)
        {
            Debug.LogWarning("Cannot perform player action: game over or not player's turn");
            return;
        }

        if (player.IsStunned)
        {
            LogBattle("Oyuncu sersemledi ve bu turu kaçırdı!");
            player.ProcessStatusEffects();
            EndPlayerTurn();
            return;
        }

        if (!player.CanPerformAction(actionIndex))
        {
            LogBattle("Yeterli enerji yok!");
            return;
        }

        // Process status effects before action
        player.ProcessStatusEffects();

        // Perform the action
        string actionName = player.GetActionName(actionIndex);
        player.PerformAction(actionIndex, opponent);
        
        // Play sound effect
        AudioManager.Instance?.PlaySFX("action");

        // Check for game over
        if (!isGameOver)
        {
            EndPlayerTurn();
        }
    }

    /// <summary>
    /// End player's turn and start opponent's turn
    /// </summary>
    private void EndPlayerTurn()
    {
        isPlayerTurn = false;
        
        // Give energy regeneration
        player.RestoreEnergy(energyRegenPerTurn);
        
        // Start opponent turn after a short delay
        Invoke(nameof(StartOpponentTurn), 1f);
    }

    /// <summary>
    /// Start opponent's turn
    /// </summary>
    private void StartOpponentTurn()
    {
        if (isGameOver) return;

        OnOpponentTurnStart?.Invoke();

        if (opponent.IsStunned)
        {
            LogBattle("Rakip sersemledi ve bu turu kaçırdı!");
            opponent.ProcessStatusEffects();
            EndOpponentTurn();
            return;
        }

        // Process status effects
        opponent.ProcessStatusEffects();

        // Get AI decision
        int actionIndex = opponent.GetAIDecision(player);
        
        // Ensure the chosen action can be performed
        int attempts = 4;
        while (!opponent.CanPerformAction(actionIndex) && attempts > 0)
        {
            actionIndex = (actionIndex + 1) % 4;
            attempts--;
        }

        // Perform the action
        string actionName = opponent.GetActionName(actionIndex);
        opponent.PerformAction(actionIndex, player);
        
        // Play sound effect
        AudioManager.Instance?.PlaySFX("action");

        // Check for game over
        if (!isGameOver)
        {
            EndOpponentTurn();
        }
    }

    /// <summary>
    /// End opponent's turn and start next round
    /// </summary>
    private void EndOpponentTurn()
    {
        // Give energy regeneration
        opponent.RestoreEnergy(energyRegenPerTurn);
        
        // Start next turn
        turnCount++;
        isPlayerTurn = true;
        
        OnTurnChanged?.Invoke(turnCount);
        OnPlayerTurnStart?.Invoke();
        LogBattle("Tur " + turnCount + " - Oyuncu sırası");
    }

    /// <summary>
    /// Called when player dies
    /// </summary>
    private void OnPlayerDeath()
    {
        if (isGameOver) return;
        
        isGameOver = true;
        LogBattle("Oyuncu yenildi! Rakip kazandı!");
        OnGameOver?.Invoke("Rakip");
        AudioManager.Instance?.PlaySFX("lose");
    }

    /// <summary>
    /// Called when opponent dies
    /// </summary>
    private void OnOpponentDeath()
    {
        if (isGameOver) return;
        
        isGameOver = true;
        LogBattle("Rakip yenildi! Oyuncu kazandı!");
        OnGameOver?.Invoke("Oyuncu");
        AudioManager.Instance?.PlaySFX("win");
    }

    /// <summary>
    /// Log battle message
    /// </summary>
    private void LogBattle(string message)
    {
        Debug.Log("[Battle] " + message);
        OnBattleLog?.Invoke(message);
    }

    /// <summary>
    /// Restart the current game
    /// </summary>
    public void RestartGame()
    {
        InitializeGame();
    }

    /// <summary>
    /// Return to main menu
    /// </summary>
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Quit the game
    /// </summary>
    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
