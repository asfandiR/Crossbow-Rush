using System.Collections.Generic;
using UnityEngine;

public class TowerUnlockManager : MonoBehaviour
{
    [Header("Tower Types")]
    [SerializeField] private List<Tower> AllTowers = new List<Tower>(); // Все возможные типы башен
    [SerializeField] private List<bool> initiallyUnlocked = new List<bool>(); // Which towers are initially unlocked

    [Header("Unlock Settings")]
    [SerializeField] private int baseUnlockCost = 10; // Base cost for unlocking

    private List<bool> towersLockState = new List<bool>();
    private readonly List<int> TowerLevels = new List<int>();
    public static TowerUnlockManager Instance { get; private set; }
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
        // Initialize the lock states based on the Inspector settings
        towersLockState = new List<bool>(initiallyUnlocked);

        // Activate the GameObjects for initially unlocked towers
        for (int i = 0; i < towersLockState.Count; i++)
        {
            if (towersLockState[i] && i < AllTowers.Count)
            {
                AllTowers[i].gameObject.SetActive(true);
            }
        }

        // Subscribe to the tower upgrade event to check for new unlocks
        GlobalEventManager.Instance.OnTowerUpgraded.AddListener(CheckForUnlock);
    }

    private void CheckForUnlock(Tower tower)
    {
        //Debug.Log("Checking for tower unlocks...");
        if (AreAllUnlockedTowersMaxLevel())
        {
            UnlockNextAvailableTower();
        }
    }

    /// <summary>
    /// Checks if all currently unlocked towers have reached their maximum level.
    /// </summary>
    private bool AreAllUnlockedTowersMaxLevel()
    {
        for (int i = 0; i < towersLockState.Count; i++)
        {
            // Only check towers that are currently unlocked
            if (towersLockState[i] && !AllTowers[i].TowerStats.IsAtMaxLevel)
            {
                return false; // Found an unlocked tower that is not at max level
            }
        }
        return true; // All unlocked towers are at max level
    }

    /// <summary>
    /// Finds the next locked tower in the list and unlocks it.
    /// </summary>
    private void UnlockNextAvailableTower()
    {
        for (int i = 0; i < towersLockState.Count; i++)
        {
            if (!towersLockState[i])
            {
                towersLockState[i] = true;
                if (i < AllTowers.Count)
                {
                    AllTowers[i].gameObject.SetActive(true);
                }
                // NOTE: This currently unlocks for free. To add a cost, you would integrate MoneyManager here.
                //Debug.Log($"Unlocked new tower type: {AllTowers[i].name}");
                break; // Unlock only one tower at a time
            }
        }
    }

    /// <summary>
    /// Calculates the cost to unlock the next tower.
    /// NOTE: This method is not currently used by the automatic unlock logic.
    /// </summary>
    public int GetNextUnlockCost()
    {
        int unlockedCount = 0;
        foreach (bool u in towersLockState)
        {
            if (u) unlockedCount++;
        }
        return baseUnlockCost + unlockedCount - 1; // Example: Base 10, each subsequent unlock costs +1
    }

    /// <summary>
    /// Counts how many towers have been built (are not in the "NotBuilded" state).
    /// Useful for other systems that have dependencies on tower count.
    /// </summary>
    public int GetBuiltTowerCount()
    {
        int builtCount = 0;
        foreach (Tower u in AllTowers)
        {
            if (u.TowerStates.CurrentBuildingState == TowerStates.BuildingState.Builded||u.TowerStates.CurrentBuildingState == TowerStates.BuildingState.MaxLevel) 
            {
               builtCount++;    
            }
        }
       return builtCount;
    }

    /// <summary>
    /// Returns a list containing the current level of each tower type.
    /// </summary>
    public List<int> GetTowerLevels()
    {
        TowerLevels.Clear();
        foreach (Tower tower in AllTowers)
        {
            TowerLevels.Add(tower.TowerStats.CurrentLevel);
        }
        return TowerLevels;
    }
}