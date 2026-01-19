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
        HandleAttack();
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

    private void HandleAttack()
    {
        if (agent == null ) return;

        Vector3 direction = (agent.nextPosition - transform.position).normalized;
        if (direction == Vector3.zero) direction = transform.forward;

        Wall wallToAttack = TargetFinder.FindWallBlockingPath(transform.position, direction, attackRange);

        if (wallToAttack != null && wallToAttack.IsAlive)
        {
            ProcessAttack(wallToAttack);
            return;
        }

        // Если стены нет, проверяем базу
        if (mainBase != null && mainBase.IsAlive)
        {
            float dist = Vector3.Distance(transform.position, mainBase.transform.position);
            float rangeCheck = attackRange;

            if (baseCollider != null)
            {
                // Если есть коллайдер, считаем расстояние до ближайшей точки (точнее для больших баз)
                dist = Vector3.Distance(transform.position, baseCollider.ClosestPoint(transform.position));
            }
            else
            {
                // Если коллайдера нет, даем запас, так как база может быть большой
                rangeCheck += 2.0f; 
            }

            if (dist <= rangeCheck)
            {
                ProcessAttack(mainBase);
                return;
            }
        }

        if (!isDead) agent.enabled = true;
        agent.SetDestination(mainBase.transform.position);
    }

    private void ProcessAttack(HealthSystem target)
    {
        if (isDead) return;
        agent.enabled = false;

        attackTimer += Time.deltaTime;
        if (attackTimer >= attackCooldown)
        {
            AttackTarget(target);
            Debug.Log("Attacking " + target.gameObject.name);
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