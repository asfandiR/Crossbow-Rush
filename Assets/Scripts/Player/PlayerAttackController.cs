using UnityEngine;
using UnityEngine.Events;

public class PlayerAttackController : MonoBehaviour
{
    public UnityEvent OnAttack;

    [Header("Projectile")]
    [SerializeField] private UpgradableStats playerStats;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
public Vector3 LastTargetPosition {get;private set; }
    private float nextFireTime;
public bool hasTarget{get;private set;}
    private void Start()
    {
        if (playerStats == null)
        {
            playerStats = GetComponent<UpgradableStats>();
        }
    }

    private void Update()
    {
        // 1. Поиск лучшей цели
        Enemy target = TargetFinder.FindBestEnemyTarget(transform.position, playerStats.CurrentRange);
        if (target != null)
        {
            hasTarget=true;
            LastTargetPosition = target.transform.position;
            RotateTowards( LastTargetPosition);
            TryToShoot(target.transform);
        }
        else
        {
            hasTarget=false;
        }
    }
     private void RotateTowards(Vector3 target)
    {
       transform.LookAt(target);
       /* float roty=Mathf.Atan2(target.x,target.y)*Mathf.Rad2Deg;
        Vector3 currentkRotEu= transform.rotation.eulerAngles;
        Vector3 newRotEu =new Vector3 (currentkRotEu.x,roty,currentkRotEu.z);
        transform.rotation=Quaternion.Euler(newRotEu);*/
    }
    private void TryToShoot(Transform target)
    {
        // Проверка времени перезарядки
        if (Time.time < nextFireTime) return;
        
            Shoot(target);
            nextFireTime = Time.time + 1f / playerStats.CurrentFireRate; // Обновляем время следующей атаки
        
    }
    
    /// <summary>
    /// Создает снаряд и устанавливает ему цель.
    /// </summary>
    private void Shoot(Transform target)
    {
        OnAttack?.Invoke();
        // 1. Создаем снаряд
        GameObject boltGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        
        // 2. Устанавливаем цель в скрипте Projectile
        if (boltGO.TryGetComponent(out Projectile bolt))
        {
            
            bolt.SetTargetAndDamage(target, playerStats.CurrentDamage);
        }
        else
        {
            Destroy(boltGO);
            Debug.LogError("Projectile prefab must have a Projectile component!");
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Визуализация радиуса атаки в редакторе
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, playerStats.CurrentRange);
    }
}