using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

// Интерфейс, который говорит: "Этому можно нанести урон"
public interface IDamageable
{
    void TakeDamage(float amount);
}

// Базовый класс здоровья для Врагов и Базы
public class HealthSystem : MonoBehaviour, IDamageable
{
    [Header("Settings")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

public bool IsAlive => currentHealth > 0;
public float CurrentHealth { get { return currentHealth;} private set { currentHealth = value; } }
public float MaxHealth { get { return maxHealth;} private set { maxHealth = value; } }
public float HealtPercentage => currentHealth / maxHealth;
    // События для UI или смерти (например, проиграть анимацию)
    public UnityEvent OnDeath;
    public UnityEvent<float> OnDamageTaken;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }
public void SetStartingHealth(float health)
    {
        maxHealth = health;
        currentHealth = maxHealth;
    }
    public virtual void TakeDamage(float amount)
    {
       // Debug.Log("İ Damaged"+ gameObject.name);
        currentHealth -= amount;
        OnDamageTaken?.Invoke(currentHealth / maxHealth); // Для полоски HP
//Debug.Log(gameObject.name+"I Damaged");
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        OnDeath?.Invoke();
       /* Destroy(this);
        Destroy(gameObject,3f); // По умолчанию просто уничтожаем объект*/
    }
}