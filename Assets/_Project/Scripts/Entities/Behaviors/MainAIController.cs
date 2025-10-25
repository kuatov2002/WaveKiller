using MoreMountains.Feedbacks;
using UnityEngine;
using Pathfinding;

public class MainAIController : MonoBehaviour
{
    [Header("Feedbacks")]
    public MMFeedbacks hitFeedback;
    
    // === Настройки ===
    [Header("Detection")]
    public float detectionRadius = 30f;

    [Header("Movement")]
    public float moveSpeed = 4f; // Будет применяться к RichAI
    public float stopDistance = 2f;
    public bool rotateTowardsEnemy = true; // RichAI может сам это делать

    [Header("Attack")]
    public float attackRange = 3f;
    public float attackAngle = 90f;
    public float damage = 10f;
    public float attackCooldown = 2f;

    // === Компоненты ===
    private RichAI richAI;
    private AIDestinationSetter destinationSetter;
    private Transform closestEnemy;
    [SerializeField] private Animator animator;

    // === Внутренние состояния ===
    private float lastAttackTime;
    private bool isAttacking = false;
    void Start()
    {
        richAI = GetComponent<RichAI>();
        destinationSetter = GetComponent<AIDestinationSetter>();

        if (richAI == null || destinationSetter == null)
        {
            Debug.LogError("RichAI or AIDestinationSetter not found on " + gameObject.name, this);
            enabled = false;
            return;
        }

        if (animator == null)
            Debug.LogWarning("Animator not assigned on " + gameObject.name, this);

        // Настройка RichAI
        richAI.maxSpeed = moveSpeed;
        richAI.rotationSpeed = rotateTowardsEnemy ? richAI.rotationSpeed : 0f; // или отключи в инспекторе
    }

    void Update()
    {
        FindClosestEnemy();

        if (closestEnemy == null || !IsEnemyValid())
        {
            // Нет цели — остановиться
            SetDestination(transform);
            SetSpeed(0f);
            return;
        }

        if (isAttacking)
        {
            // Ждём окончания атаки
            return;
        }

        if (IsInAttackRange())
        {
            HandleAttack();
            SetDestination(transform); // Остановиться у цели
        }
        else
        {
            HandleMovement();
        }
    }

    void FindClosestEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius);
        closestEnemy = null;
        float minSqrDistance = float.MaxValue;
        Vector3 myPos = transform.position;

        foreach (var col in colliders)
        {
            if (col.CompareTag("Enemy"))
            {
                Transform enemyTransform = col.transform;
                float sqrDist = (enemyTransform.position - myPos).sqrMagnitude;
                if (sqrDist < minSqrDistance)
                {
                    minSqrDistance = sqrDist;
                    closestEnemy = enemyTransform;
                }
            }
        }
    }

    bool IsEnemyValid()
    {
        return closestEnemy != null && closestEnemy.gameObject.activeInHierarchy;
    }

    bool IsInAttackRange()
    {
        if (!IsEnemyValid()) return false;

        Vector3 toEnemy = closestEnemy.position - transform.position;
        toEnemy.y = 0;
        float distanceSqr = toEnemy.sqrMagnitude;
        if (distanceSqr > attackRange * attackRange) return false;

        float angle = Vector3.Angle(transform.forward, toEnemy);
        return angle <= attackAngle * 0.5f;
    }

    void HandleAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        SetDestination(transform);
        richAI.canMove = false; // Блокируем движение
        animator?.SetTrigger("Attack");
        lastAttackTime = Time.time;
        isAttacking = true;
    }

    void HandleMovement()
    {
        if (!IsEnemyValid()) return;

        // Устанавливаем цель для AIDestinationSetter
        SetDestination(closestEnemy);

        // Проверяем, достаточно ли близко (учитывая stopDistance)
        float distance = Vector3.Distance(transform.position, closestEnemy.position);
        if (distance <= stopDistance)
        {
            SetSpeed(0f);
        }
        else
        {
            SetSpeed(richAI.velocity.magnitude);
        }
    }

    void SetDestination(Transform target)
    {
        destinationSetter.target = target;
    }

    void SetSpeed(float speed)
    {
        // RichAI использует maxSpeed, но аниматору передаём значение
        animator?.SetFloat("Speed", speed);
        // Опционально: обновить maxSpeed (если нужно динамически менять)
        // richAI.maxSpeed = speed;
    }

    public void ApplyDamage()
    {
        if (!IsEnemyValid()) return;

        IDamageable damageable = closestEnemy.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
            hitFeedback?.PlayFeedbacks(); // например, звук удара или частицы от AI
        }
    }

    public void OnAttackAnimationFinished()
    {
        isAttacking = false;
        richAI.canMove = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (closestEnemy != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, closestEnemy.position);
            Gizmos.DrawWireSphere(closestEnemy.position, attackRange);
        }
    }
}