using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : HealthSystem
{
    public Transform CoinSpawnPoint;

    [SerializeField] private int coinsToDrop = 1;
    [SerializeField] private float attackRange = 0.3f;
    [SerializeField]private Transform target;
    
    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float attackDamage = 1f;

    [Header("Animation Settings")]
    [SerializeField] private Animator enemyAnimator;
    [SerializeField] private float moveAnimationSpeed = 3.0f;

    public int CoinsToDrop { get { return coinsToDrop; } private set { coinsToDrop = value; } }

    private NavMeshAgent agent;
    private bool isDead = false;
    private float attackTimer = 0f;

    private static readonly int IsMovingHash = Animator.StringToHash("Walk");
    private static readonly int IsDeadHash = Animator.StringToHash("IsDead");

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (enemyAnimator == null) 
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

        if (wallToAttack == null)
        {
            if (!isDead) agent.enabled = true;
            return;
        }

        if (wallToAttack.IsAlive)
        {
            if (isDead)
            return; 
            agent.enabled = false;

            attackTimer += Time.deltaTime;
            if (attackTimer >= attackCooldown)
            {
                AttackTarget(wallToAttack);
                Debug.Log("Attacking " + wallToAttack.gameObject.name);
            }
        }
    }

    private void AttackTarget(Wall wall)
    {
        if (wall != null && wall.IsAlive)
        {
            wall.TakeDamage(attackDamage);
        }
        attackTimer = 0f;
    }

    private void HandleAnimation()
    {
        if (enemyAnimator == null) return;

        if (agent != null && agent.velocity.magnitude > 0.1f)
        {
            enemyAnimator.SetFloat(IsMovingHash, moveAnimationSpeed);
        }
        else
        {
            enemyAnimator.SetFloat(IsMovingHash, 0f);
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

        if (enemyAnimator != null)
        {
            enemyAnimator.SetFloat(IsMovingHash, 0f);
            enemyAnimator.SetBool(IsDeadHash, true);
        }

        base.Die();
        GetComponent<Collider>().enabled = false;
        GlobalEventManager.Instance.OnEnemyDied.Invoke(this);
       
        Destroy(gameObject, 2f); // Уничтожаем объект через 2 секунды, чтобы дать время анимации смерти проиграться
    }
}