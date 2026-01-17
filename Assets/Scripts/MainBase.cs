using UnityEngine;

public class MainBase : HealthSystem
{
    protected override void Die()
    {
        base.Die();
        Debug.Log("GAME OVER! Base destroyed.");
        // Здесь вызовем GameManager.Instance.GameOver();
    }
}