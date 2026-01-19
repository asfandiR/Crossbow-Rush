using UnityEngine;
using UnityEngine.UI;

public class MainBase : HealthSystem
{
    [SerializeField]private Image healtBar;

    protected override void Start()
    {
        base.Start();
        healtBar.fillAmount = HealtPercentage;
    base.OnDamageTaken.AddListener(UpdateHealthBar);
    }
    private void UpdateHealthBar(float damage)
    {
        healtBar.fillAmount = HealtPercentage;
    }
    protected override void Die()
    {
        base.Die();
        Debug.Log("GAME OVER! Base destroyed.");
        // Здесь вызовем GameManager.Instance.GameOver();
    }
    private void OnDisable()
    {
        base.OnDamageTaken.RemoveListener(UpdateHealthBar);
    }

}