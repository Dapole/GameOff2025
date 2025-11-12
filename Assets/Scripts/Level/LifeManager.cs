using UnityEngine;

public class LifeManager : MonoBehaviour
{
    [SerializeField] private int _maxLives = 10;
    [SerializeField] private int _currentLives;

    public int CurrentLives => _currentLives;
    public int MaxLives => _maxLives;

    private void Start()
    {
        _currentLives = _maxLives;
        NotifyLivesChanged();
    }

    public void Initialize(int lives)
    {
        _maxLives = lives;
        _currentLives = lives;
        NotifyLivesChanged();
    }

    public void TakeDamage(int damage)
    {
        _currentLives = Mathf.Max(0, _currentLives - damage);
        NotifyLivesChanged();

        if (_currentLives <= 0)
        {
            EventBus.Publish(new GameOverEvent { IsVictory = false });
        }
    }

    public void Heal(int amount)
    {
        _currentLives = Mathf.Min(_maxLives, _currentLives + amount);
        NotifyLivesChanged();
    }

    private void NotifyLivesChanged()
    {
        EventBus.Publish(new LivesChangedEvent
        {
            CurrentLives = _currentLives,
            MaxLives = _maxLives
        });
    }
}

