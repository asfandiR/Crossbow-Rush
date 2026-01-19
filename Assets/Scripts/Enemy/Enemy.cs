using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : HealthSystem
{
    private static readonly int IsMovingHash = Animator.StringToHash("Walk");
    private static readonly int IsDeadHash = Animator.StringToHash("IsDead");
    public Transform CoinSpawnPoint;

    [SerializeField] private int coinsToDrop = 1;
    [SerializeField] private float attackRange = 0.3f;
    [SerializeField] private Transform target;
    
    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float attackDamage = 1f;

    [Header("Animation Settings")]
    [SerializeField] private Animator animatorController;
    [SerializeField] private float moveAnimationSpeed = 3.0f;


    private NavMeshAgent agent;
    private bool isDead = false;
    private float attackTimer = 0f;
    private MainBase mainBase;
    private Collider baseCollider;
    private HealthSystem currentAttackTarget;

    // --- Optimization ---
    private float findTargetTimer;
    private const float FindTargetInterval = 0.2f; // Как часто искать цель (в секундах)

    public int CoinsToDrop { get { return coinsToDrop; } private set { coinsToDrop = value; } }
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (animatorController == null) 
        Debug.LogError("Please assign Animator pidaras");
    }

    private void Update()
    {
        if (isDead) return;

        // Оптимизация: поиск цели выполняется по таймеру, а не каждый кадр
        findTargetTimer -= Time.deltaTime;
        if (findTargetTimer <= 0f)
        {
            UpdateAttackTarget();
            findTargetTimer = FindTargetInterval;
        }

        // Логика атаки выполняется каждый кадр, если есть цель
        if (currentAttackTarget != null && currentAttackTarget.IsAlive)
        {
            ProcessAttack(currentAttackTarget);
        }
        else if (agent.isStopped) // Если цели нет, а агент остановлен - возобновляем движение
        {
            agent.isStopped = false;
        }

        HandleAnimation();
    }

    public void SetMainBase(Transform baseTransform)
    {
        if (baseTransform != null)
        {
            mainBase = baseTransform.GetComponent<MainBase>();
            if (mainBase != null) baseCollider = mainBase.GetComponent<Collider>();
        }
        if (agent != null && agent.isOnNavMesh)
        {
            agent.SetDestination(baseTransform.position);
        }
    }

    /// <summary>
    /// Периодически обновляет цель для атаки (стена или база). Вызывается по таймеру.
    /// </summary>
    private void UpdateAttackTarget()
    {
        if (agent == null || !agent.isOnNavMesh) return;

        // Используем steeringTarget для более точного определения направления движения
        Vector3 direction = (agent.steeringTarget - transform.position).normalized;
        if (direction == Vector3.zero) direction = transform.forward;

        // Рекомендация: здесь нужно использовать LayerMask для стен (слой 10).
        // Это изменение нужно вносить в сам TargetFinder.FindWallBlockingPath.
        Wall wallToAttack = TargetFinder.FindWallBlockingPath(transform.position, direction, attackRange);

        if (wallToAttack != null && wallToAttack.IsAlive)
        {
            currentAttackTarget = wallToAttack;
            return;
        }

        // Если стены на пути нет, проверяем, не находимся ли мы в радиусе атаки базы
        if (mainBase != null && mainBase.IsAlive)
        {
            float dist;
            float rangeCheck = attackRange;

            if (baseCollider != null)
            {
                dist = Vector3.Distance(transform.position, baseCollider.ClosestPoint(transform.position));
            }
            else
            {
                dist = Vector3.Distance(transform.position, mainBase.transform.position);
                rangeCheck += 2.0f; 
            }

            if (dist <= rangeCheck)
            {
                currentAttackTarget = mainBase;
                return;
            }
        }

        // Если в радиусе атаки целей нет
        currentAttackTarget = null;
    }

    private void ProcessAttack(HealthSystem target)
    {
        if (isDead) return;
        // Оптимизация: используем isStopped вместо отключения агента
        if (!agent.isStopped) agent.isStopped = true;

        attackTimer += Time.deltaTime;
        if (attackTimer >= attackCooldown)
        {
            AttackTarget(target);
            // Оптимизация: логирование только в редакторе, чтобы не создавать мусор в билде
            #if UNITY_EDITOR
            Debug.Log("Attacking " + target.gameObject.name + " | " + target.CurrentHealth);
            #endif
        }
    }

    private void AttackTarget(HealthSystem target)
    {
        if (target != null && target.IsAlive)
        {
            target.TakeDamage(attackDamage);
        }
        attackTimer = 0f;
    }

    private void HandleAnimation()
    {
        if (animatorController == null) return;

        if (agent != null && agent.velocity.magnitude > 0.1f)
        {
            animatorController.SetFloat(IsMovingHash, moveAnimationSpeed);
        }
        else
        {
            animatorController.SetFloat(IsMovingHash, 0f);
        }
    }

    protected override void Die()
    {
        if (isDead) return;
        isDead = true;
        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        if (animatorController != null)
        {
            animatorController.SetFloat(IsMovingHash, 0f);
            animatorController.SetBool(IsDeadHash, true);
        }

        base.Die();
        GetComponent<Collider>().enabled = false;
        GlobalEventManager.Instance.OnEnemyDied.Invoke(this);
       
        Destroy(gameObject, 2f); // Уничтожаем объект через 2 секунды, чтобы дать время анимации смерти проиграться
    }
}