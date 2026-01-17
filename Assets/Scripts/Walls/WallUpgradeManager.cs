using TMPro;
using UnityEngine;
using UnityEngine.Events;
public class WallUpgradeManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI upgradePriceText;
    [SerializeField]private WallUpgradeTrigger upgradeTrigger;
    [SerializeField] private WallGroup[] wallGroups;
    [SerializeField] private float baseUpgradeCost = 20f;
    [SerializeField] private float priceIncreaseMultiplierPerLevel = 0.5f;

    [Header("Unlock Settings")]
    [Tooltip("Towers required to unlock each level. Element 0 is for the first wall, Element 1 for the second, etc.")]
    [SerializeField] private int[] towersRequiredForLevel; 
    private int builtTowersCount = 0;

    public UnityEvent<int> OnWallUpgraded;

    private int currentWallLevel=0;
    private void Awake()
    {
        
        if (upgradeTrigger == null)
        {
            Debug.LogError("WallUpgradeManager: WallUpgradeTrigger component is missing.");
        }
    }
    private void Start()
    {
        upgradeTrigger.OnWallUpgradeTriggered.AddListener(UpgradeAllWallGroups);
        UpdatePriceText();

        if (GlobalEventManager.Instance != null)
        {
            GlobalEventManager.Instance.OnTowerBuilded.AddListener(OnTowerBuilt);
        }
    }
    private void OnDestroy()
    {
        if (GlobalEventManager.Instance != null)
        {
            GlobalEventManager.Instance.OnTowerBuilded.RemoveListener(OnTowerBuilt);
        }
    }
    private void OnTowerBuilt(Tower tower)
    {
        builtTowersCount++;
    }
    public float CurrentPriceForUpgrade()
    {
            if(currentWallLevel==0)
            {
                return baseUpgradeCost;
            }
            return baseUpgradeCost + (baseUpgradeCost * priceIncreaseMultiplierPerLevel * currentWallLevel);
        
    }
    private void UpgradeAllWallGroups()
    {
        if(wallGroups.Length == 0 || currentWallLevel>=wallGroups[0].MaxWallLevel)
        {
           // Debug.Log("All wall groups are already at max level.");
            return;
        }

        // Check Tower Requirement
        if (towersRequiredForLevel != null && currentWallLevel < towersRequiredForLevel.Length)
        {
            int required = towersRequiredForLevel[currentWallLevel];
            if (builtTowersCount < required)
            {
                Debug.Log($"Upgrade Locked! You need {required} towers. Currently built: {builtTowersCount}");
                return;
            }
        }

        if(MoneyManager.Instance.TrySpend(Mathf.RoundToInt(CurrentPriceForUpgrade())))
        {
        foreach (var group in wallGroups)
        {
            group.UpgradeWalls();
        }
        OnWallUpgraded?.Invoke(currentWallLevel);
            currentWallLevel++;
            UpdatePriceText();
    }
    }
    private void UpdatePriceText()
    {
        if (upgradePriceText != null)
            upgradePriceText.text = CurrentPriceForUpgrade().ToString();
    }
}