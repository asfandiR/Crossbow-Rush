using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(UpgradableStats))]
[RequireComponent(typeof(TowerAttackLogic))]
[RequireComponent(typeof(TowerAnimator))]
[RequireComponent(typeof(TowerStates))]
[RequireComponent(typeof(TowerSkinController))]
public class Tower : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] private UpgradableStats towerStats;
    [SerializeField] private int buildPrice;
    private TowerStates towerStates;
  

    // Публичное свойство для получения стоимости постройки/улучшения башни
    public int BuildPrice {get { return buildPrice; } private set { buildPrice = value; } }
public TowerStates TowerStates {get { return towerStates; } private set { towerStates = value; } }
public UpgradableStats TowerStats {get { return towerStats; } private set { towerStats = value; } }
    // Событие, которое оповещает о том, что башня улучшена (для UI или GameManager)
    public UnityEvent<int> OnTowerUpgraded;
    public UnityEvent<Tower> OnTowerBuilded;

    // Публичные свойства для доступа к текущим параметрам
    public int CurrentUpgradeCost => towerStats.CurrentUpgradeCost();

    private void Awake()
    {
        towerStates = GetComponent<TowerStates>();
        gameObject.SetActive(false); // Изначально башня неактивна
        if (gameObject.activeSelf)
        {
            if (towerStats.IsAtMaxLevel)
            {
                towerStates.CurrentBuildingState = TowerStates.BuildingState.MaxLevel;
            }
            else
            {
                towerStates.CurrentBuildingState = TowerStates.BuildingState.Builded;
            }
        }
        else
        {
            towerStates.CurrentBuildingState = TowerStates.BuildingState.NotBuilded;
        }
    }

    /// <summary>
    /// Метод, вызываемый, например, кнопкой в UI
    /// </summary>
    public bool TryUpgrade()
    {
        if (towerStats.TryIncreaseLevel() )
        { 
            //Debug.Log($"Tower upgraded to Level {towerStats.CurrentLevel}. New Damage: {towerStats.CurrentDamage:F1}");
           GlobalEventManager.Instance.OnTowerUpgraded.Invoke(this);
            OnTowerUpgraded?.Invoke(towerStats.CurrentLevel); // Вызываем событие
            return true;
        }
        else
        {
             return false;
           
        }
    }
}