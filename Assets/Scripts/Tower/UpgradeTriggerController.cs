using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(ProgressBarController))]
public class UpgradeTriggerController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Tower towerPrefab;
    [SerializeField] private GameObject progressBarParent;
    [SerializeField] private float startSensitivity = 2f;
    private TowerStates towerStates;
    private UpgradableStats towerStats;

    private float localTimer = 0f;
    private TowerBuilder towerBuilder;
     private ProgressBarController progressController;
    
    public UnityEvent OnUpgradeTriggered;

    private void Awake()
    {
        towerStates = towerPrefab.GetComponent<TowerStates>();
        towerStats = towerPrefab.GetComponent<UpgradableStats>();

        if(towerStates==null)
        Debug.LogError("UpgradeTriggerController: towerStates is null");
        
        if (towerPrefab == null)
            Debug.LogError("UpgradeTriggerController: towerPrefab is null");
        if (towerStats == null)
            Debug.LogError("UpgradeTriggerController: UpgradableStats is null on towerPrefab");
        if (progressBarParent == null)
            Debug.LogError("UpgradeTriggerController: progressBarParent is null");
        towerBuilder = new TowerBuilder(towerPrefab);
        progressController = GetComponent<ProgressBarController>();
        if (progressController == null)
            Debug.LogError("UpgradeTriggerController: ProgressBarController is missing.");
    }

    private void Start()
    {
        switch (towerStates.CurrentBuildingState)
        {
            case TowerStates.BuildingState.NotBuilded:

                progressController.SetPriceText(towerPrefab.BuildPrice);
                break;
            case TowerStates.BuildingState.Builded:
                progressController.SetPriceText(towerPrefab.CurrentUpgradeCost);
                break;
            case TowerStates.BuildingState.MaxLevel:
                // nothing
                break;
        }
        progressController.SetProgress(0f);
    }

    public void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        int targetCost = 0;
        bool canUpgrade = false;

        switch (towerStates.CurrentBuildingState)
        {
            case TowerStates.BuildingState.NotBuilded:
                targetCost = towerPrefab.BuildPrice;
                canUpgrade = true;
                break;
            case TowerStates.BuildingState.Builded:
                targetCost = towerPrefab.CurrentUpgradeCost;
                canUpgrade = true;
                break;
            case TowerStates.BuildingState.MaxLevel:
                canUpgrade = false;
                progressBarParent.SetActive(false);
                break;
        }

        if (canUpgrade)
        {
            localTimer += Time.deltaTime;
           
            if(localTimer >= startSensitivity)
            {
                float progress = Mathf.Clamp01(localTimer /( startSensitivity*2));
                progressController.SetProgress(progress);
                
            }
            if (localTimer >= startSensitivity*2)
            {
                if (MoneyManager.Instance.TrySpend(targetCost))
                {
               CoinPaymentAnimation.Instance.PlayPaymentAnimation(other.transform, transform, targetCost);
                    CompleteTransaction();
                    OnUpgradeTriggered?.Invoke();
                }
                else
                {
                    localTimer = startSensitivity; // Держим таймер на максимуме, пока игрок не уйдет или не накопит денег
                }
            }
        }
    }

    private void CompleteTransaction()
    {
        localTimer = 0f;
        progressController.SetProgress(0f);

        if (towerStates.CurrentBuildingState == TowerStates.BuildingState.NotBuilded)
        {
            towerStates.CurrentBuildingState = TowerStates.BuildingState.Builded;
            towerBuilder.BuildTower();
        }
        else if (towerStates.CurrentBuildingState == TowerStates.BuildingState.Builded)
        {
            towerBuilder.TryUpgrade();
            if (towerStats != null && towerStats.IsAtMaxLevel)
            {
                towerStates.CurrentBuildingState = TowerStates.BuildingState.MaxLevel;
            }
        }

        // Update price text for the next level (or hide it if max level logic handles it elsewhere)
        if (towerStates.CurrentBuildingState != TowerStates.BuildingState.MaxLevel)
        {
            progressController.SetPriceText(towerPrefab.CurrentUpgradeCost);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            localTimer = 0f;
            progressController.SetProgress(0f);
        }
    }  
}