using UnityEngine;
using System.Collections;

public abstract class Projectile : MonoBehaviour
{
    [SerializeField] protected float speed = 25f;       // Скорость полета
    [SerializeField] protected float lifeTime = 5f;
    [SerializeField] protected float damageRadius = 0.5f; // Расстояние, при котором снаряд считается попавшим

    protected Transform target; // Цель, которую преследуем
    protected float damage;     // Урон, взятый из UpgradeManager

    /// <summary>
    /// Ключ для возврата объекта в пул. Должен устанавливаться тем, кто спавнит снаряд.
    /// </summary>
    public string PoolKey { get; set; }
    private Coroutine returnToPoolCoroutine;

    // ОБЩЕСТВЕННЫЙ МЕТОД: вызывается HeroTurretLogic при создании снаряда
    public virtual void SetTargetAndDamage(Transform newTarget, float newDamage)
    {
        damage = newDamage;
        target = newTarget;
        
        // Получаем урон из менеджера улучшений (УРОН ГЕРОЯ)
        if (damage == 0) damage = 10f; 
        
        OnTargetSet();

        // Оптимизация: вместо уничтожения по времени, запускаем корутину для возврата в пул
        if (returnToPoolCoroutine != null)
        {
            StopCoroutine(returnToPoolCoroutine);
        }
        returnToPoolCoroutine = StartCoroutine(ReturnToPoolAfterTime(lifeTime));
    }

    protected virtual void OnTargetSet() { }

    protected virtual void Update()
    {
        // 1. Проверяем, существует ли цель
        if (target == null)
        {
            // Если цель исчезла, возвращаем снаряд в пул
            ReturnToPool();
            return;
        }

        Move();
    }

    protected abstract void Move();

    // В отличие от прошлого варианта, теперь используем HitTarget вместо OnTriggerEnter,
    // так как снаряд может "проскочить" сквозь коллайдеры на высокой скорости.
    protected virtual void HitTarget()
    {
        // Сначала наносим урон, если цель еще жива
        if (target != null && target.TryGetComponent(out Enemy enemy))
        {
            enemy.TakeDamage(damage);
        }
        
        // Оптимизация: возвращаем снаряд в пул вместо уничтожения
        ReturnToPool();
    }

    private IEnumerator ReturnToPoolAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnToPool();
    }

    protected void ReturnToPool()
    {
        // Останавливаем корутину, если она была запущена, чтобы избежать двойного возврата в пул
        if (returnToPoolCoroutine != null)
        {
            StopCoroutine(returnToPoolCoroutine);
            returnToPoolCoroutine = null;
        }

        // Проверяем наличие пула и ключа
        if (ObjectPool.Instance != null && !string.IsNullOrEmpty(PoolKey))
        {
            ObjectPool.Instance.ReturnToPool(PoolKey, gameObject);
        }
        else
        {
            Destroy(gameObject); // Запасной вариант
        }
    }
}