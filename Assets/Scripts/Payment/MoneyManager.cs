using UnityEngine;

public class MoneyManager:MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }
    [SerializeField]private int coins = 0;
    public  int CurrentCoinBalance { get { return coins; } private set { coins = value; } }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        if(GlobalEventManager.Instance!=null)
        GlobalEventManager.Instance.OnGoldChanged.Invoke(coins);
    }

    public  void AddCoins(int amount)
    {
        if (amount <= 0) return;
        coins += amount;
       if(GlobalEventManager.Instance!=null)
        GlobalEventManager.Instance.OnGoldChanged.Invoke(coins);
    }

    public  bool TrySpend(int amount)
    {
        if (amount <= 0) return false;
        if (coins >= amount)
        {
            coins -= amount;
            if(GlobalEventManager.Instance!=null)
        GlobalEventManager.Instance.OnGoldChanged.Invoke(coins);
            return true;
        }
        return false;
    }

   /* public static void Set(int amount)
    {
        coins = Mathf.Max(0, amount);
    }*/
}
