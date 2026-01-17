using UnityEngine;

/// <summary>
/// Общий компонент для игрока/башни: хранит базовые статы и уровень улучшения.
/// Публичные свойства возвращают значение по формуле: base * currentLevel * increasePerLevel
/// </summary>
public class UpgradableStats : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] private float baseDamage = 15f;
    [SerializeField] private float baseFireRate = 1f;
    [SerializeField] private float baseRange = 10f;

    [Header("Leveling")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int maxLevel = 5;

    [Header("Per-level multipliers (used in formula base * level * multiplier)")]
    [SerializeField] private float damageIncreasePerLevel = 0.20f;
    [SerializeField] private float fireRateIncreasePerLevel = 0.01f;
    //[SerializeField] private float rangeIncreasePerLevel = 0.01f;
    [SerializeField] private float priceIncreasePerLevel = 0.25f;

    [Header("Upgrade cost base")]
    [SerializeField] private int baseUpgradeCost = 5;

    // Публичные геттеры (используют формулу base * currentLevel * increasePerLevel)
    public float CurrentDamage => baseDamage+baseDamage * currentLevel * damageIncreasePerLevel;
    public float CurrentFireRate => baseFireRate+baseFireRate * currentLevel * fireRateIncreasePerLevel;
    public float CurrentRange => baseRange;

    public int CurrentLevel => currentLevel;
    public int MaxLevel => maxLevel;
    public bool IsAtMaxLevel => currentLevel >= maxLevel;

    // Попробовать повысить уровень (без списания золота) — возвращает true при успешном повышении
    public bool TryIncreaseLevel()
    {
        if (IsAtMaxLevel) return false;
        currentLevel++;
        return true;
    }
    public int CurrentUpgradeCost ()
    {
        if (IsAtMaxLevel) return 0;
        return Mathf.RoundToInt(baseUpgradeCost + baseUpgradeCost * currentLevel * priceIncreasePerLevel);
    }
    // Установить уровень (без превышения лимитов)
    public void SetLevel(int level)
    {
        currentLevel = Mathf.Clamp(level, 1, maxLevel);
    }
}