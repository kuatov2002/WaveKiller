using MoreMountains.Feedbacks;
using UnityEngine;
using Pathfinding;

public class MageAIController : MonoBehaviour
{
    [Header("Feedbacks")]
    public MMFeedbacks fireballFeedback;
    public MMFeedbacks aoeFeedback;

    [Header("Detection")]
    public float detectionRadius = 30f;
    public int aoeEnemyThreshold = 4; // Сколько врагов рядом — чтобы кастовать AoE

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float stopDistance = 8f; // Маг держится подальше
    public bool rotateTowardsEnemy = true;

    [Header("Attack - Fireball")]
    public GameObject fireballPrefab;
    public Transform fireballSpawnPoint;
    public float fireballSpeed = 20f;
    public float fireballDamage = 10f;
    public float attackRange = 20f;
    public float attackAngle = 90f;
    public float attackCooldown = 2f;

    [Header("Attack - AoE Explosion")]
    public float aoeRadius = 6f;
    public float aoeKnockbackForce = 10f;
    public float aoeDamage = 25f;
    public float aoeCooldown = 5f;

    // === Компоненты ===
    private RichAI richAI;
    private AIDestinationSetter destinationSetter;
    private Transform closestEnemy;
    [SerializeField] private Animator animator;

    // === Внутренние состояния ===
    private float lastAttackTime;
    private float lastAoeTime;
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

        richAI.maxSpeed = moveSpeed;
        if (!rotateTowardsEnemy)
            richAI.rotationSpeed = 0f;
    }

    void Update()
    {
        FindClosestEnemy();

        if (closestEnemy == null || !IsEnemyValid())
        {
            SetDestination(transform);
            SetSpeed(0f);
            return;
        }

        if (isAttacking) return;

        // Проверяем, можно ли использовать AoE
        if (ShouldCastAoE())
        {
            CastAoE();
        }
        else if (IsInAttackRange())
        {
            HandleFireballAttack();
            SetDestination(transform);
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

    bool ShouldCastAoE()
    {
        if (Time.time - lastAoeTime < aoeCooldown) return false;

        Collider[] colliders = Physics.OverlapSphere(transform.position, aoeRadius);
        int enemyCount = 0;
        foreach (var col in colliders)
        {
            if (col.CompareTag("Enemy"))
                enemyCount++;
        }

        return enemyCount >= aoeEnemyThreshold;
    }

    void HandleFireballAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        SetDestination(transform);
        richAI.canMove = false;
        animator?.SetTrigger("Attack");
        lastAttackTime = Time.time;
        isAttacking = true;
    }

    void HandleMovement()
    {
        if (!IsEnemyValid()) return;

        SetDestination(closestEnemy);

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
        animator?.SetFloat("Speed", speed);
    }

    // === Анимационные события ===

    public void ShootFireball()
    {
        if (!IsEnemyValid() || fireballPrefab == null || fireballSpawnPoint == null) return;

        // Направление — туда, куда смотрит маг (а не к врагу)
        Vector3 direction = transform.forward;
        direction.y = 0; // опционально: летит строго горизонтально
        direction.Normalize();

        // Устанавливаем поворот фаербола так, чтобы он смотрел в направлении полёта
        Quaternion fireballRotation = Quaternion.LookRotation(direction);

        GameObject fb = Instantiate(fireballPrefab, fireballSpawnPoint.position, fireballRotation);

        Fireball fireball = fb.GetComponent<Fireball>();
        if (fireball != null)
        {
            fireball.Initialize(direction, fireballSpeed);
            fireball.damage = fireballDamage;
        }

        fireballFeedback?.PlayFeedbacks();
    }

    public void CastAoE()
    {
        if (Time.time - lastAoeTime < aoeCooldown) return;

        aoeFeedback?.PlayFeedbacks();

        Collider[] colliders = Physics.OverlapSphere(transform.position, aoeRadius);
        foreach (var col in colliders)
        {
            if (col.CompareTag("Enemy"))
            {
                // Наносим урон
                IDamageable damageable = col.GetComponent<IDamageable>();
                if (damageable != null)
                    damageable.TakeDamage(aoeDamage);

                // Отбрасываем через Rigidbody
                Rigidbody rb = col.attachedRigidbody;
                if (rb != null)
                {
                    Vector3 direction = (col.transform.position - transform.position).normalized;
                    direction.y = 0.5f; // немного вверх для эффекта взрыва
                    rb.AddForce(direction * aoeKnockbackForce, ForceMode.Impulse);
                }
            }
        }

        lastAoeTime = Time.time;
    }

    public void OnAttackAnimationFinished()
    {
        isAttacking = false;
        richAI.canMove = true;
    }

    // === Визуализация в редакторе ===
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);

        if (closestEnemy != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, closestEnemy.position);
            Gizmos.DrawWireSphere(closestEnemy.position, attackRange);
        }
    }
}