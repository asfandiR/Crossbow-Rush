using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    /// <summary>
    /// Корутина, которая выполняет спавн всех групп во волне.
    /// Группы спавнятся одновременно (параллельно).
    /// </summary>
    public IEnumerator SpawnWaveCoroutine(Wave wave, Transform spawnPoint, Transform mainBase)
    {
         
        if (wave == null || spawnPoint == null) yield break;

        foreach (var group in wave.groups)
        {
            StartCoroutine(SpawnGroupCoroutine(group, spawnPoint, mainBase));
            // Ждем, пока вся группа заспавнится, перед началом следующей
            
        }
        yield return new WaitForSeconds(wave.timeBeforeNextWave); 
    }

    /// <summary>
    /// Корутина для спавна одной группы врагов с задержкой между спавнами.
    /// </summary>
    private IEnumerator SpawnGroupCoroutine(EnemyGroup group, Transform spawnPoint, Transform mainBase)
    {
        for (int i = 0; i < group.count; i++)
        {
            if (group.enemyPrefab != null && group.enemyPrefab.GetComponent<Enemy>() != null)
            {      
                SpawnEnemyAtPoint(group.enemyPrefab, spawnPoint, mainBase);
                 yield return new WaitForSeconds(group.spawnDelay);              
            }
            
        }
    }

    private void SpawnEnemyAtPoint(GameObject enemyPrefab, Transform spawnPoint, Transform mainBase)
    {
        if (enemyPrefab != null && spawnPoint != null)
        {
             GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
             newEnemy.GetComponent<Enemy>().SetMainBase(mainBase);
            
        }
    }
}