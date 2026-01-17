using UnityEngine;

public class TowerBuilder
{
    private Tower towerPrefab;
    public TowerBuilder(Tower towerPrefab)
    {
        this.towerPrefab = towerPrefab;

    }

    // Activate initial tower and return next upgrade cost (keeps original logic)
    public void BuildTower()
    {
        if (towerPrefab != null)
        {
            GlobalEventManager.Instance.OnTowerBuilded.Invoke(towerPrefab);
            towerPrefab.OnTowerBuilded?.Invoke(towerPrefab);
        }
        
    }

    public void TryUpgrade()
    {
        if (towerPrefab != null){
            towerPrefab.TryUpgrade();
            towerPrefab.OnTowerUpgraded?.Invoke(towerPrefab.TowerStats.CurrentLevel);
        } 
    }
}