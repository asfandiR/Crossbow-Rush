using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CoinPaymentAnimation : MonoBehaviour
{
    public static CoinPaymentAnimation Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int initialPoolSize = 20;

    [Header("Animation Settings")]
    [SerializeField] private int maxCoinsToSpawn = 10;
    [SerializeField] private float animationDuration = 2f;
    [SerializeField] private float arcHeight = 2.0f;
    [SerializeField] private float magnetStrength = 12f; // сила притяжения

    private Queue<GameObject> coinPool = new Queue<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializePool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject coin = Instantiate(coinPrefab, transform);
            coin.SetActive(false);
            coinPool.Enqueue(coin);
        }
    }

    public void PlayPaymentAnimation(Transform startTransformPosition, Transform endTransform, int amount)
    {
        StartCoroutine(SpawnCoinsRoutine(startTransformPosition, endTransform, amount));
    }

    private IEnumerator SpawnCoinsRoutine(Transform start, Transform target, int amount)
    {
        int coinsCount = Mathf.Clamp(amount, 1, maxCoinsToSpawn);
        if (amount < 5) coinsCount = amount;

        float density = Mathf.Lerp(1f, 0.35f, (float)coinsCount / maxCoinsToSpawn);

        float spawnInterval = animationDuration / coinsCount;
        float flightDuration = animationDuration - spawnInterval * (coinsCount - 1);

        for (int i = 0; i < coinsCount; i++)
        {
            SpawnSingleCoin(start, target, animationDuration);
            yield return null;//new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnSingleCoin(Transform start, Transform target, float flightDuration)
    {
        if (coinPool.Count == 0)
        {
            GameObject newCoin = Instantiate(coinPrefab, transform);
            newCoin.SetActive(false);
            coinPool.Enqueue(newCoin);
        }

        GameObject coin = coinPool.Dequeue();
        coin.transform.position = start.position;
        coin.SetActive(true);

        StartCoroutine(MoveCoinRoutine(coin, start, target, flightDuration));
    }

    private IEnumerator MoveCoinRoutine(GameObject coin, Transform startPos, Transform target, float duration)
    {
        float elapsed = 0f;
        Vector3 actualStart = startPos.position ;

        while (elapsed < duration)
        {
            if (coin == null || !coin.activeSelf) yield break;

            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            Vector3 targetPos = target.position;

            Vector3 pos = Vector3.Lerp(actualStart, targetPos, t);
            pos.y += arcHeight * 4 * t * (1 - t);

            // Магнит — притяжение усиливается к концу
            pos = Vector3.Lerp(pos, targetPos, Time.deltaTime * magnetStrength * t);

            coin.transform.position = pos;
            yield return null;
        }

        coin.transform.position = target.position;
        coin.SetActive(false);
        coinPool.Enqueue(coin);
    }
}
