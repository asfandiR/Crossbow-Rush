
using UnityEngine;
[RequireComponent(typeof(WallUpgradeManager))]
public class WallManager : MonoBehaviour
{
  [SerializeField] private WallGroup[] wallGroups;
private WallUpgradeManager wallUpgradeManager;
  private void Start()
  {
    if(wallUpgradeManager==null)
      wallUpgradeManager = GetComponent<WallUpgradeManager>();
      wallUpgradeManager.OnWallUpgraded.AddListener(HandleWallBuilt);
      DeactivateAllWalls();
  }     
  private void HandleWallBuilt(int wallLevel)
  {
    int index = wallLevel;
    //Debug.Log("WallManager detected wall built. Total built walls: " + index);
    ActivateWallGroup(index); // Активируем группу стен соответствующую уровню
  }
    private void ActivateWallGroup(int index)
    {
        if (index < 0 || index >= wallGroups.Length)
        {
           // Debug.Log("WallGroup index out of range: " + index);
            return;
        }
    
        wallGroups[index].ActivateWalls();
    }   
    private void DeactivateAllWalls()
    {
        foreach (var group in wallGroups)
        {
            group.DeactivateWalls();
        }
    }
}
