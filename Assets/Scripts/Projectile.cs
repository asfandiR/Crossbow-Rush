using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 25f;       // Скорость полета
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private float damageRadius = 0.5f; // Расстояние, при котором снаряд считается попавшим

    private Transform target; // Цель, которую преследуем
    private float damage;     // Урон, взятый из UpgradeManager

    // ОБЩЕСТВЕННЫЙ МЕТОД: вызывается HeroTurretLogic при создании снаряда
    public void SetTargetAndDamage(Transform newTarget, float newDamage)
    {
        damage = newDamage;
        SetTarget(newTarget);
    }

    private void SetTarget(Transform newTarget)
    {
        target = newTarget;
        
        // Получаем урон из менеджера улучшений (УРОН ГЕРОЯ)
        if (damage==0)
        {
            damage = 10f; 
        }
        if (target != null)
        {
            RotateTowards(new Vector2(target.position.x, target.position.z));
        }
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        // 1. Проверяем, существует ли цель
        if (target == null)
        {
            // Если цель исчезла (умерла), просто летим вперед до самоуничтожения
           Destroy(gameObject);
            return;
        }

       transform.position = Vector3.MoveTowards(
            transform.position, 
            target.position, 
            speed * Time.deltaTime
        );
        
        if (Vector3.Distance(transform.position, target.position) <= damageRadius)
        {
            HitTarget();
        }
    }
private void RotateTowards(Vector2 target)
    {
        Vector2 direction = target - new Vector2(transform.position.x, transform.position.z);
        float roty = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        Vector3 currentRotEu = transform.rotation.eulerAngles;
        Vector3 newRotEu = new Vector3(currentRotEu.x, roty, currentRotEu.z);
        transform.rotation = Quaternion.Euler(newRotEu);
    }
    // В отличие от прошлого варианта, теперь используем HitTarget вместо OnTriggerEnter,
    // так как снаряд может "проскочить" сквозь коллайдеры на высокой скорости.
    private void HitTarget()
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