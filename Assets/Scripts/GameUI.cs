using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Game UI Controller
/// Handles all in-game UI elements including:
/// - Player and Opponent health/energy bars
/// - Action buttons
/// - Battle log
/// - Game over panel
/// </summary>
public class GameUI : MonoBehaviour
{
    [Header("Player UI")]
    [SerializeField] private Slider playerHealthBar;
    [SerializeField] private Slider playerEnergyBar;
    [SerializeField] private Text playerHealthText;
    [SerializeField] private Text playerEnergyText;
    [SerializeField] private Text playerStatusText;

    [Header("Opponent UI")]
    [SerializeField] private Slider opponentHealthBar;
    [SerializeField] private Slider opponentEnergyBar;
    [SerializeField] private Text opponentHealthText;
    [SerializeField] private Text opponentEnergyText;
    [SerializeField] private Text opponentStatusText;

    [Header("Action Buttons")]
    [SerializeField] private Button[] actionButtons = new Button[4];
    [SerializeField] private Text[] actionButtonTexts = new Text[4];

    [Header("Game Info")]
    [SerializeField] private Text turnText;
    [SerializeField] private Text turnIndicatorText;

    [Header("Battle Log")]
    [SerializeField] private Text battleLogText;
    [SerializeField] private ScrollRect battleLogScrollRect;
    [SerializeField] private int maxLogLines = 10;

    [Header("Game Over Panel")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Text gameOverText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    private List<string> battleLogMessages = new List<string>();
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
        
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found!");
            return;
        }

        // Subscribe to events
        if (gameManager.Player != null)
        {
            gameManager.Player.OnHealthChanged.AddListener(UpdatePlayerHealth);
            gameManager.Player.OnEnergyChanged.AddListener(UpdatePlayerEnergy);
        }

        if (gameManager.Opponent != null)
        {
            gameManager.Opponent.OnHealthChanged.AddListener(UpdateOpponentHealth);
            gameManager.Opponent.OnEnergyChanged.AddListener(UpdateOpponentEnergy);
        }

        gameManager.OnTurnChanged.AddListener(UpdateTurnDisplay);
        gameManager.OnBattleLog.AddListener(AddBattleLog);
        gameManager.OnGameOver.AddListener(ShowGameOver);
        gameManager.OnPlayerTurnStart.AddListener(OnPlayerTurnStart);
        gameManager.OnOpponentTurnStart.AddListener(OnOpponentTurnStart);

        // Setup action buttons
        SetupActionButtons();

        // Setup game over buttons
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartClicked);
        }
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        }

        // Hide game over panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // Initialize UI
        InitializeUI();

        // Play battle music
        AudioManager.Instance?.PlayMusic("battle");
    }

    /// <summary>
    /// Initialize UI with current values
    /// </summary>
    private void InitializeUI()
    {
        if (gameManager.Player != null)
        {
            UpdatePlayerHealth(gameManager.Player.CurrentHealth, gameManager.Player.MaxHealth);
            UpdatePlayerEnergy(gameManager.Player.CurrentEnergy, gameManager.Player.MaxEnergy);
        }

        if (gameManager.Opponent != null)
        {
            UpdateOpponentHealth(gameManager.Opponent.CurrentHealth, gameManager.Opponent.MaxHealth);
            UpdateOpponentEnergy(gameManager.Opponent.CurrentEnergy, gameManager.Opponent.MaxEnergy);
        }

        UpdateTurnDisplay(1);
        ClearBattleLog();
    }

    /// <summary>
    /// Setup action buttons with player action names
    /// </summary>
    private void SetupActionButtons()
    {
        if (gameManager.Player == null) return;

        for (int i = 0; i < actionButtons.Length; i++)
        {
            if (actionButtons[i] != null)
            {
                int actionIndex = i; // Capture for closure
                actionButtons[i].onClick.AddListener(() => OnActionButtonClicked(actionIndex));
                
                // Set button text
                string actionName = gameManager.Player.GetActionName(i);
                int energyCost = gameManager.Player.GetActionEnergyCost(i);
                
                if (actionButtonTexts[i] != null)
                {
                    actionButtonTexts[i].text = actionName + "\n(" + energyCost + " Enerji)";
                }
            }
        }
    }

    /// <summary>
    /// Called when an action button is clicked
    /// </summary>
    private void OnActionButtonClicked(int actionIndex)
    {
        if (gameManager != null && gameManager.IsPlayerTurn && !gameManager.IsGameOver)
        {
            gameManager.PlayerAction(actionIndex);
        }
    }

    /// <summary>
    /// Update player health display
    /// </summary>
    private void UpdatePlayerHealth(int current, int max)
    {
        if (playerHealthBar != null)
        {
            playerHealthBar.maxValue = max;
            playerHealthBar.value = current;
        }
        if (playerHealthText != null)
        {
            playerHealthText.text = "Can: " + current + "/" + max;
        }
    }

    /// <summary>
    /// Update player energy display
    /// </summary>
    private void UpdatePlayerEnergy(int current, int max)
    {
        if (playerEnergyBar != null)
        {
            playerEnergyBar.maxValue = max;
            playerEnergyBar.value = current;
        }
        if (playerEnergyText != null)
        {
            playerEnergyText.text = "Enerji: " + current + "/" + max;
        }

        // Update action button interactability
        UpdateActionButtonStates();
    }

    /// <summary>
    /// Update opponent health display
    /// </summary>
    private void UpdateOpponentHealth(int current, int max)
    {
        if (opponentHealthBar != null)
        {
            opponentHealthBar.maxValue = max;
            opponentHealthBar.value = current;
        }
        if (opponentHealthText != null)
        {
            opponentHealthText.text = "Can: " + current + "/" + max;
        }
    }

    /// <summary>
    /// Update opponent energy display
    /// </summary>
    private void UpdateOpponentEnergy(int current, int max)
    {
        if (opponentEnergyBar != null)
        {
            opponentEnergyBar.maxValue = max;
            opponentEnergyBar.value = current;
        }
        if (opponentEnergyText != null)
        {
            opponentEnergyText.text = "Enerji: " + current + "/" + max;
        }
    }

    /// <summary>
    /// Update action button states based on player energy
    /// </summary>
    private void UpdateActionButtonStates()
    {
        if (gameManager == null || gameManager.Player == null) return;

        for (int i = 0; i < actionButtons.Length; i++)
        {
            if (actionButtons[i] != null)
            {
                bool canPerform = gameManager.IsPlayerTurn && 
                                  !gameManager.IsGameOver && 
                                  gameManager.Player.CanPerformAction(i);
                actionButtons[i].interactable = canPerform;
            }
        }
    }

    /// <summary>
    /// Update turn display
    /// </summary>
    private void UpdateTurnDisplay(int turn)
    {
        if (turnText != null)
        {
            turnText.text = "Tur: " + turn;
        }
    }

    /// <summary>
    /// Called when player turn starts
    /// </summary>
    private void OnPlayerTurnStart()
    {
        if (turnIndicatorText != null)
        {
            turnIndicatorText.text = "Senin Sıran!";
            turnIndicatorText.color = Color.green;
        }
        SetActionButtonsEnabled(true);
        UpdateActionButtonStates();
        UpdateStatusDisplays();
    }

    /// <summary>
    /// Called when opponent turn starts
    /// </summary>
    private void OnOpponentTurnStart()
    {
        if (turnIndicatorText != null)
        {
            turnIndicatorText.text = "Rakip Oynuyor...";
            turnIndicatorText.color = Color.red;
        }
        SetActionButtonsEnabled(false);
        UpdateStatusDisplays();
    }

    /// <summary>
    /// Enable/disable action buttons
    /// </summary>
    private void SetActionButtonsEnabled(bool enabled)
    {
        foreach (var button in actionButtons)
        {
            if (button != null)
            {
                button.interactable = enabled;
            }
        }
    }

    /// <summary>
    /// Update status effect displays
    /// </summary>
    private void UpdateStatusDisplays()
    {
        if (gameManager.Player != null && playerStatusText != null)
        {
            string status = "";
            if (gameManager.Player.IsDefending) status += "[Savunma] ";
            if (gameManager.Player.IsPoisoned) status += "[Zehirli] ";
            if (gameManager.Player.IsStunned) status += "[Sersem] ";
            playerStatusText.text = status;
        }

        if (gameManager.Opponent != null && opponentStatusText != null)
        {
            string status = "";
            if (gameManager.Opponent.IsDefending) status += "[Savunma] ";
            if (gameManager.Opponent.IsPoisoned) status += "[Zehirli] ";
            if (gameManager.Opponent.IsStunned) status += "[Sersem] ";
            opponentStatusText.text = status;
        }
    }

    /// <summary>
    /// Add message to battle log
    /// </summary>
    private void AddBattleLog(string message)
    {
        battleLogMessages.Add("[" + System.DateTime.Now.ToString("HH:mm:ss") + "] " + message);
        
        // Keep only last N messages
        while (battleLogMessages.Count > maxLogLines)
        {
            battleLogMessages.RemoveAt(0);
        }

        // Update display
        if (battleLogText != null)
        {
            battleLogText.text = string.Join("\n", battleLogMessages);
        }

        // Scroll to bottom
        if (battleLogScrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            battleLogScrollRect.verticalNormalizedPosition = 0f;
        }
    }

    /// <summary>
    /// Clear battle log
    /// </summary>
    private void ClearBattleLog()
    {
        battleLogMessages.Clear();
        if (battleLogText != null)
        {
            battleLogText.text = "";
        }
    }

    /// <summary>
    /// Show game over panel
    /// </summary>
    private void ShowGameOver(string winner)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        
        if (gameOverText != null)
        {
            if (winner == "Oyuncu")
            {
                gameOverText.text = "TEBRİKLER!\nKazandınız!";
                gameOverText.color = Color.green;
            }
            else
            {
                gameOverText.text = "OYUN BİTTİ\nKaybettiniz!";
                gameOverText.color = Color.red;
            }
        }

        SetActionButtonsEnabled(false);
    }

    /// <summary>
    /// Called when restart button is clicked
    /// </summary>
    private void OnRestartClicked()
    {
        AudioManager.Instance?.PlaySFX("button");
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        ClearBattleLog();
        gameManager?.RestartGame();
        InitializeUI();
    }

    /// <summary>
    /// Called when main menu button is clicked
    /// </summary>
    private void OnMainMenuClicked()
    {
        AudioManager.Instance?.PlaySFX("button");
        gameManager?.ReturnToMainMenu();
    }
}
