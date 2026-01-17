using System.Collections.Generic;
using UnityEngine;

public class CoinMagnet : MonoBehaviour
{
    [Header("Magnet Settings")]
    [SerializeField] private float magnetRadius = 5f;
    [SerializeField] private float attractSpeed = 12f;
    
    [Header("Optimization")]
    [Tooltip("Как часто проверять наличие монет вокруг (в секундах). Чем выше, тем меньше нагрузка.")]
    [SerializeField] private float scanInterval = 0.5f; 

    private float scanTimer;
    
    // List to track coins currently being pulled
    private readonly List<Transform> attractedCoins = new List<Transform>();
    
    // Pre-allocated buffer for physics checks
    private static readonly Collider[] _colliderBuffer = new Collider[50]; 

    void Update()
    {
        // 1. Таймер сканирования (чтобы не спамить физикой каждый кадр)
        scanTimer -= Time.deltaTime;
        if (scanTimer <= 0)
        {
            ScanForCoins();
            scanTimer = scanInterval;
        }

        // 2. Движение найденных монет (выполняется плавно каждый кадр)
        MoveCoins();
    }

    private void ScanForCoins()
    {
        // Обновляем список монет поблизости без аллокаций
        int count = Physics.OverlapSphereNonAlloc(transform.position, magnetRadius, _colliderBuffer);
        
        for (int i = 0; i < count; i++)
        {
            Collider col = _colliderBuffer[i];
            if (col != null && col.CompareTag("Coin"))
            {
                // Если монеты еще нет в списке, добавляем
                if (!attractedCoins.Contains(col.transform))
                {
                    attractedCoins.Add(col.transform);
                }
            }
            // Очищаем ссылку в буфере
            _colliderBuffer[i] = null;
        }
    }

    private void MoveCoins()
    {
        // Итерируемся в обратном порядке, чтобы безопасно удалять элементы
        for (int i = attractedCoins.Count - 1; i >= 0; i--)
        {
            Transform coin = attractedCoins[i];

            // Проверка на null (если монета уже собрана или удалена)
            if (coin == null || !coin.gameObject.activeSelf) 
            {
                attractedCoins.RemoveAt(i);
                continue;
            }

            // Тянем монету к игроку
            Vector3 targetPos = transform.position + Vector3.up * 1f;
            coin.position = Vector3.MoveTowards(
                coin.position, 
                targetPos, 
                attractSpeed * Time.deltaTime
            );

            // Сбор монеты (используем SqrMagnitude для оптимизации)
            if (Vector3.SqrMagnitude(coin.position - targetPos) < 0.04f) // 0.2f * 0.2f
            {
                CollectCoin(coin.gameObject);
                attractedCoins.RemoveAt(i);
            }
        }
    }

    private void CollectCoin(GameObject coinObj)
    {
        if (MoneyManager.Instance != null) 
            MoneyManager.Instance.AddCoins(1);
        
        if (ObjectPool.Instance != null)
        {
            ObjectPool.Instance.ReturnToPool("Coin", coinObj);
        }
        else
        {
            Destroy(coinObj);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, magnetRadius);
    }
}