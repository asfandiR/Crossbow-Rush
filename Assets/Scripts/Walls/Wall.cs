using UnityEngine.UI;
using UnityEngine;
public class Wall : HealthSystem
{
    [SerializeField] private GameObject[] wallVisuals;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private GameObject healthBarImageParent;
    private int currentLevel = 0;
    public int MaxWallLevel { get { return wallVisuals.Length; } }
    protected override void Start()
    {
        base.Start();
        // Установить maxHealth если нужно
        healthBarImageParent.SetActive(false);
        currentLevel = 1;
    }

    public override void TakeDamage(float damage)
    {
        healthBarImageParent.SetActive(true);
        base.TakeDamage(damage);
        if (!IsAlive)
        {
            Die();
        }
        UpdateHealthUI();
    }

    protected override void Die()
    {
        base.Die();
        // Логика разрушения стены
       // Destroy(gameObject);
         DisableVisuals();
        DeactivateHealthUI();
        gameObject.SetActive(false);
    }
    private  void UpdateHealthUI()
    {
        if (healthBarImage != null)
        {
            healthBarImage.fillAmount = HealtPercentage;
        }
    }
    public void UpgradeWall()
    {
        if (currentLevel >= MaxWallLevel)
        {
            Debug.Log("Wall is already at max level.");
            return;
        }
        // Логика улучшения стены, например, увеличение maxHealth
        SetStartingHealth(MaxHealth + 50); // Увеличиваем максимальное здоровье на 50 при каждом улучшении
        UpdateHealthUI();
        currentLevel++;
        ChangeWallVisual();
    }
    private void ChangeWallVisual()
    {
        if (wallVisuals == null || wallVisuals.Length == 0)
            return;
        // Логика изменения визуала стены в зависимости от currentLevel
        for (int i = 0; i < wallVisuals.Length; i++)
        {
            wallVisuals[i].SetActive(i == currentLevel - 1);
        }
    }
    private void DeactivateHealthUI()
    {
        healthBarImageParent.SetActive(false);
    }
    private void DisableVisuals()
    {
        foreach (var visual in wallVisuals)
        {
            visual.SetActive(false);
        }
    }
}