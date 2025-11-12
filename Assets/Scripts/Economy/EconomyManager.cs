using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    [SerializeField] private int _currentGold = 100;

    public int CurrentGold => _currentGold;

    private void Start()
    {
        NotifyGoldChanged();
    }

    public void Initialize(int startingGold)
    {
        _currentGold = startingGold;
        NotifyGoldChanged();
    }

    public void AddGold(int amount)
    {
        _currentGold += amount;
        NotifyGoldChanged();
    }

    public bool SpendGold(int amount)
    {
        if (_currentGold >= amount)
        {
            _currentGold -= amount;
            NotifyGoldChanged();
            return true;
        }
        return false;
    }

    private void NotifyGoldChanged()
    {
        EventBus.Publish(new GoldChangedEvent
        {
            CurrentGold = _currentGold
        });
    }
}


