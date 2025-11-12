using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _goldTextTMP;
    [SerializeField] private TextMeshProUGUI _livesTextTMP;
    [SerializeField] private TextMeshProUGUI _waveTextTMP;
    [SerializeField] private Button _startWaveButton;

    [Header("Managers")]
    [SerializeField] private WaveManager _waveManager;
    [SerializeField] private EconomyManager _economyManager;
    [SerializeField] private LifeManager _lifeManager;

    private void Start()
    {
        if (_waveManager == null)
        {
            _waveManager = FindFirstObjectByType<WaveManager>();
        }

        if (_economyManager == null)
        {
            _economyManager = FindFirstObjectByType<EconomyManager>();
        }

        if (_lifeManager == null)
        {
            _lifeManager = FindFirstObjectByType<LifeManager>();
        }

        if (_startWaveButton != null)
        {
            _startWaveButton.onClick.AddListener(OnStartWaveClicked);
        }

        SubscribeToEvents();
        UpdateUI();
    }

    private void SubscribeToEvents()
    {
        EventBus.Subscribe<GoldChangedEvent>(OnGoldChanged);
        EventBus.Subscribe<LivesChangedEvent>(OnLivesChanged);
        EventBus.Subscribe<WaveStartedEvent>(OnWaveStarted);
        EventBus.Subscribe<WaveCompletedEvent>(OnWaveCompleted);
    }

    private void OnStartWaveClicked()
    {
        if (_waveManager != null && !_waveManager.IsWaveActive)
        {
            _waveManager.StartWave();
        }
    }

    private void OnGoldChanged(GoldChangedEvent evt)
    {
        UpdateGoldText(evt.CurrentGold);
    }

    private void OnLivesChanged(LivesChangedEvent evt)
    {
        UpdateLivesText(evt.CurrentLives, evt.MaxLives);
    }

    private void OnWaveStarted(WaveStartedEvent evt)
    {
        UpdateWaveText(evt.WaveNumber);
        UpdateStartButtonState(false);
    }

    private void OnWaveCompleted(WaveCompletedEvent evt)
    {
        UpdateWaveText(_waveManager != null ? _waveManager.CurrentWaveNumber : 0);
        UpdateStartButtonState(true);
    }

    private void UpdateUI()
    {
        if (_economyManager != null)
        {
            UpdateGoldText(_economyManager.CurrentGold);
        }

        if (_lifeManager != null)
        {
            UpdateLivesText(_lifeManager.CurrentLives, _lifeManager.MaxLives);
        }

        if (_waveManager != null)
        {
            UpdateWaveText(_waveManager.CurrentWaveNumber);
            UpdateStartButtonState(!_waveManager.IsWaveActive);
        }
    }

    private void UpdateGoldText(int gold)
    {
        if (_goldTextTMP != null)
        {
            _goldTextTMP.text = $"Gold: {gold}";
        }
    }

    private void UpdateLivesText(int lives, int maxLives)
    {
        if (_livesTextTMP != null)
        {
            _livesTextTMP.text = $"Lives: {lives}/{maxLives}";
        }
    }

    private void UpdateWaveText(int waveNumber)
    {
        if (_waveTextTMP != null)
        {
            int totalWaves = _waveManager != null ? _waveManager.TotalWaves : 0;
            _waveTextTMP.text = $"Wave: {waveNumber}/{totalWaves}";
        }
    }

    private void UpdateStartButtonState(bool enabled)
    {
        if (_startWaveButton != null)
        {
            _startWaveButton.interactable = enabled;
            string buttonText = enabled ? "Start Wave" : "Wave In Progress...";
            
            // Try TextMeshPro first, fallback to Text
            var textTMP = _startWaveButton.GetComponentInChildren<TextMeshProUGUI>();
            if (textTMP != null)
            {
                textTMP.text = buttonText;
            }
            else
            {
                var text = _startWaveButton.GetComponentInChildren<Text>();
                if (text != null)
                {
                    text.text = buttonText;
                }
            }
        }
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<GoldChangedEvent>(OnGoldChanged);
        EventBus.Unsubscribe<LivesChangedEvent>(OnLivesChanged);
        EventBus.Unsubscribe<WaveStartedEvent>(OnWaveStarted);
        EventBus.Unsubscribe<WaveCompletedEvent>(OnWaveCompleted);
    }
}

