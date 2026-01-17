using UnityEngine;
[RequireComponent(typeof(TowerStates))]
public class TowerSkinController : MonoBehaviour
{
    [SerializeField] private GameObject[] towerSkins;
    [SerializeField] private GameObject towerSkinsParent;

    private UpgradableStats towerStates;
    private Tower tower;
    private void Awake()
    {
        if (towerStates == null)
        {
            towerStates = GetComponent<UpgradableStats>();
        }
        if (tower == null)
        {
            tower = GetComponent<Tower>();
        }
    }
    private void Start()
    {
        DeactivateAllSkins();
        DeactivateParent();
        tower.OnTowerBuilded.AddListener((tower) => ActivateParent());
        tower.OnTowerUpgraded.AddListener((tower) => ChangeSkin());
       // GlobalEventManager.Instance.OnTowerUpgraded.AddListener((level) => ChangeSkin());
        //GlobalEventManager.Instance.OnTowerBuilded.AddListener((twr) => ActivateParent());
    }
    private void DeactivateAllSkins()
    {
        if (towerSkins != null)
        {
            foreach (var skin in towerSkins)
            {
                skin.SetActive(false);
            }
        }
    }
    private void DeactivateParent()
    {
        if (towerSkinsParent != null)
        {
            towerSkinsParent.SetActive(false);
        }
    }
private void ActivateParent()
    {
ChangeSkin();
//        Debug.Log("Activating tower skins parent.");
        if (towerSkinsParent != null )
        {
            towerSkinsParent.SetActive(true);
        }
    }
    public void ChangeSkin()
    {
        //Debug.Log("Changing tower skin based on level: " + towerStates.CurrentLevel);
        int skinIndex = towerStates.CurrentLevel - 1; // Assuming skins are 0-indexed and levels start from 1
        if (towerSkins != null && skinIndex >= 0 && skinIndex < towerSkins.Length)
        {
            for (int i = 0; i < towerSkins.Length; i++)
            {
                towerSkins[i].SetActive(i == skinIndex);
            }
        }
        else
        {
            Debug.LogWarning("Invalid skin index or skins not assigned.");
        }
    }
    private void OnDisable()
    {
        tower.OnTowerUpgraded.RemoveListener((level) => ChangeSkin());
        tower.OnTowerBuilded.RemoveListener((twr) => ActivateParent());
    }
}