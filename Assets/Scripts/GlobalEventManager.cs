using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Глобальный менеджер событий (Singleton).
/// Использует UnityEvents для оповещения других систем о ключевых событиях в игре.
/// </summary>
public class GlobalEventManager : MonoBehaviour
{
    // Singleton pattern для легкого доступа из любого места в коде:
    public static GlobalEventManager Instance { get; private set; }

    [Header("Game Events")]
    
    // 1. Событие: Враг убит. 
    // Передает объект врага, чтобы подписчик мог получить его ценность (например, золото).
    public  UnityEvent<Enemy> OnEnemyDied = new UnityEvent<Enemy>();

    // 2. Событие: Башня улучшена.
    // Передает текущий уровень, чтобы UI мог обновиться.
    public  UnityEvent<Tower> OnTowerUpgraded = new UnityEvent<Tower>();
    public  UnityEvent<Tower> OnTowerBuilded = new UnityEvent<Tower>();

    [Header("Wave Events")]
    
    // 3. Событие: Волна началась.
    // Передает индекс/номер волны.
    public readonly UnityEvent<int> OnWaveStarted = new UnityEvent<int>();

    // 4. Событие: Все волны завершены.
    public readonly UnityEvent OnAllWavesCompleted = new UnityEvent();

    [Header("Resource Events")]
    
    // 5. Событие: Золото изменилось.
    // Передает текущее количество золота.
    public readonly UnityEvent<int> OnGoldChanged = new UnityEvent<int>();


    private void Awake()
    {
        // Реализация Singleton:
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

    // --- Примеры использования ---
    
    
    
    
    // Пример для EnemyAI при смерти
    public void NotifyEnemyDied(Enemy enemy)
    {
        //OnEnemyDied?.Invoke(enemy);
    }
}