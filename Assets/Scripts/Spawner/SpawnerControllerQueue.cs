using System.Collections.Generic;
using UnityEngine;

public class SpawnerControllerQueue : MonoBehaviour
{
    [SerializeField] private Queue<SpawnerController> spawnerControllers;
    private SpawnerController currentSpawner;

    private void Awake()
    {
        SpawnerController[] allSpawners = GetComponentsInChildren<SpawnerController>();
        foreach (SpawnerController spawner in allSpawners)
        {
            spawner.gameObject.SetActive(false);
        }
        spawnerControllers = new Queue<SpawnerController>(allSpawners);
        ActivateNextSpawner();
    }

    private void ActivateNextSpawner()
    {
        if (spawnerControllers.Count > 0)
        {
            if (currentSpawner != null)
            {
                currentSpawner.gameObject.SetActive(false);
                currentSpawner.OnAllWavesCompleted.RemoveListener(ActivateNextSpawner);
            }

            currentSpawner = spawnerControllers.Dequeue();
            currentSpawner.gameObject.SetActive(true);
            currentSpawner.OnAllWavesCompleted.AddListener(ActivateNextSpawner);
        }
    }
}