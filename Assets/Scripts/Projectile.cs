using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    [SerializeField] protected float speed = 25f;       // Скорость полета
    [SerializeField] protected float lifeTime = 5f;
    [SerializeField] protected float damageRadius = 0.5f; // Расстояние, при котором снаряд считается попавшим

    protected Transform target; // Цель, которую преследуем
    protected float damage;     // Урон, взятый из UpgradeManager

    // ОБЩЕСТВЕННЫЙ МЕТОД: вызывается HeroTurretLogic при создании снаряда
    public virtual void SetTargetAndDamage(Transform newTarget, float newDamage)
    {
        damage = newDamage;
        target = newTarget;
        
        // Получаем урон из менеджера улучшений (УРОН ГЕРОЯ)
        if (damage == 0) damage = 10f; 
        
        OnTargetSet();
        Destroy(gameObject, lifeTime);
    }

    protected virtual void OnTargetSet() { }

    protected virtual void Update()
    {
        // 1. Проверяем, существует ли цель
        if (target == null)
        {
            // Если цель исчезла (умерла), просто летим вперед до самоуничтожения
           Destroy(gameObject);
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
        if (target != null && target.TryGetComponent(out IDamageable targetHealth))
        {
            targetHealth.TakeDamage(damage);
        }
        
        // Уничтожаем снаряд
        Destroy(gameObject);
    }
}