using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

public class SpawnerController : MonoBehaviour
{

    [Header("Wave Configuration")]
    [Tooltip("Список всех волн в игре.")]
    public List<Wave> waves;

    [Header("Dependencies")]
    [Tooltip("Точка, где враги будут появляться (должна совпадать с Waypoint[0] в PathManager).")]
    [SerializeField] private Transform spawnPoint;

    [Header("Events")]
    public UnityEvent<int> OnWaveStarted = new UnityEvent<int>();
    public UnityEvent OnAllWavesCompleted = new UnityEvent();

    private EnemySpawner spawner;
    private Transform mainBaseTransform;
    private int currentWaveIndex = 0;
    private bool isSpawning = false;

    private void Awake()
    {
        // При области ответственности: создаём/используем компонент спаунера рядом с контроллером.
        spawner = GetComponent<EnemySpawner>();
        if (spawner == null) spawner = gameObject.AddComponent<EnemySpawner>();
        if(mainBaseTransform==null)
        {
            GameObject mainBase = GameObject.FindGameObjectWithTag("Base");
            if(mainBase!=null)
            {
                mainBaseTransform = mainBase.transform;
            }
            else
            {
                Debug.LogError("MainBase not found in scene. Please assign the MainBase tag to the main base object.");
            }
        }
    }
    private void Start()
    {
        
  if (waves != null && waves.Count > 0&&isSpawning==false)
        {
           StartCoroutine(StartNextWaveDelayed(2f));
        }
        
    }

    // --- Управление волнами и корутины ---

    public IEnumerator StartNextWaveDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (currentWaveIndex < waves.Count)
        {
            StartWave(currentWaveIndex);
        }
        else
        {
            //Debug.Log("Все волны фазы 1 завершены!");
            OnAllWavesCompleted?.Invoke();
        }
    }

    private void StartWave(int index)
    {
        if (isSpawning) return;

        Wave currentWave = waves[index];
      //  Debug.Log($"Starting Wave {index + 1}: {currentWave.waveName}");
       
        OnWaveStarted?.Invoke(currentWaveIndex + 1);

        // Запускаем корутину спавна через локальную корутину, чтобы корректно выставить флаг isSpawning
        StartCoroutine(RunSpawnRoutine(currentWave));
    }

    private IEnumerator RunSpawnRoutine(Wave wave)
    {
        isSpawning = true;
        // Запускаем корутину спаунера и ждём её завершения
        yield return StartCoroutine(spawner.SpawnWaveCoroutine(wave, spawnPoint, mainBaseTransform));
        //yield return new WaitForSeconds(wave.group.count * group.spawnDelay);
        isSpawning = false;
        CheckWaveEnd();
    }

    // --- Обработка смерти врагов и завершение волны ---

    private void CheckWaveEnd()
    {
        if ( !isSpawning)
        {
            Debug.Log($"Wave {currentWaveIndex + 1} completed!");
            currentWaveIndex++;

            if (currentWaveIndex < waves.Count)
            {
                float delay = waves[currentWaveIndex - 1].timeBeforeNextWave;
                StartCoroutine(StartNextWaveDelayed(delay));
            }
            else
            {
                // Сохранена исходная логика: вызов метода-корутины без StartCoroutine,
                // как в оригинале (оставлено намеренно, чтобы не менять поведение).
                StartCoroutine(StartNextWaveDelayed(0f)); // Вызовет логику OnAllWavesCompleted в оригинальном коде (не запустит корутину)
            }
        }
    }
}
// --- Структуры Данных для Инспектора ---

[Serializable]
public class EnemyGroup
{
    [Tooltip("Префаб врага, который будет заспавнен.")]
    public GameObject enemyPrefab;
    
    [Tooltip("Количество врагов этого типа в данной группе.")]
    public int count;
    
    [Tooltip("Задержка между спавном каждого врага в этой группе.")]
    public float spawnDelay = 0.5f;
}

[Serializable]
public class Wave
{
    [Tooltip("Название волны (только для отладки).")]
    public string waveName;
    
    [Tooltip("Список групп врагов для этой волны.")]
    public List<EnemyGroup> groups;
    
    [Tooltip("Задержка перед началом следующей волны после окончания этой.")]
    public float timeBeforeNextWave = 10f;
}