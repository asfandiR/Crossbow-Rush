using UnityEngine;
using UnityEngine.Events;


public class TowerAttackLogic : MonoBehaviour
{
    [Header("Projectile")] 
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform towerHead;
     private TowerStates towerStates;
public UnityEvent OnAttack;

    private UpgradableStats towerStats;
    private float nextFireTime;

    private void Awake()
    {
        if(towerStates==null)
        {
            towerStates = GetComponent<TowerStates>();
        }
        if(towerStats==null)
        {
            towerStats = GetComponent<UpgradableStats>();
        }
        if(towerHead==null)
        {
            Debug.LogError("TowerHead component is missing on TowerAttackLogic.");
        }
    }

    private void Update()
    {
        switch (towerStates.CurrentBuildingState)
        {
            case TowerStates.BuildingState.NotBuilded:
                towerStates.CurrentAttackState = TowerStates.AttackState.Idle;
                return;
            case TowerStates.BuildingState.Builded:
            case TowerStates.BuildingState.MaxLevel:
                HandleAttackLogic();
                break;
        }
        // 1. Поиск цели
       
    }
   private void HandleAttackLogic()
    {
        Enemy target = TargetFinder.FindBestEnemyTarget(transform.position, towerStats.CurrentRange);

        if (target != null)
        {
            // 2. Поворот к цели
            RotateHeadTowards(target.transform);

            // 3. Автоматическая атака
            TryToShoot(target.transform);
        }
        else
        {
            towerStates.CurrentAttackState = TowerStates.AttackState.Idle;
        }
    }
    private void RotateHeadTowards(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        
        // Башни обычно поворачиваются быстро, но можно использовать Slerp:
       // towerHead.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 20f);
        towerHead.rotation = lookRotation;
    }

    private void TryToShoot(Transform target)
    {
        // Расчет fireRate берется из TowerStats
        float fireRate = towerStats.CurrentFireRate;
        if (Time.time < nextFireTime)
        {
            towerStates.CurrentAttackState = TowerStates.AttackState.Charging;
            return;
        }

        // Проверка: достаточно ли повернута башня (чтобы снаряд летел не под углом)
            towerStates.CurrentAttackState = TowerStates.AttackState.Attacking;
            Shoot(target, towerStats.CurrentDamage);
            nextFireTime = Time.time + 1f / fireRate; 
        
    }
    
   
    private void Shoot(Transform target, float damage)
    {
        GameObject boltGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        
        if (boltGO.TryGetComponent(out Projectile bolt))
        {
            // ВАЖНО: Мы должны дать снаряду знать, что урон башенный, а не геройский.
            // Для этого нужна небольшая корректировка Projectile.cs (см. ниже).

            bolt.SetTargetAndDamage(target, damage); 
            OnAttack?.Invoke();
        }
    }

    /*private void OnDrawGizmosSelected()
    {
        // Визуализация радиуса атаки башни
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, towerStats.CurrentRange);
        
    }*/
}