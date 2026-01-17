using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public static CoinSpawner Instance { get; private set; }
    [SerializeField] private GameObject coinPrefab;

    /// <summary>
    /// Создает несколько монет на основе данных из EnemyAI.
    /// Монеты спавнятся с небольшим случайным смещением, чтобы не накладывались.
    /// </summary>
     private void Awake(){
     if (Instance == null)
        {
            Instance = this;
            // Убеждаемся, что менеджер не удаляется при смене сцен (если у вас их несколько)
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            // Если уже существует, уничтожаем этот новый объект
            Destroy(gameObject);
        }
        }
    private void Start()
    {
        GlobalEventManager.Instance.OnEnemyDied.AddListener( SpawnCoins);
    }
   
    public void SpawnCoins(Enemy enemy)
    {
        if (coinPrefab == null)
        {
            Debug.LogError("CoinSpawner.coinPrefab is not set!");
            return;
        }

        for (int i = 0; i < enemy.CoinsToDrop; i++)
        {
            Vector3 offset = new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f));
            Instantiate(coinPrefab, enemy.CoinSpawnPoint.position + offset, Quaternion.identity);
        }
    }
    private void OnDisable()
    {
        GlobalEventManager.Instance.OnEnemyDied.RemoveListener( SpawnCoins);
    }
}