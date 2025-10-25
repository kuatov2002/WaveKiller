using UnityEngine;

public class MainAIController : MonoBehaviour
{
    // === Настройки ===
    [Header("Detection")]
    public float detectionRadius = 30f;

    [Header("Movement")]
    public float moveSpeed = 4f;
    public float stopDistance = 2f;
    public float rotationSpeed = 10f;
    public bool rotateTowardsEnemy = true;

    [Header("Attack")]
    public float attackRange = 3f;
    public float attackAngle = 90f;
    public float damage = 10f;
    public float attackCooldown = 2f;

    // === Внутренние состояния ===
    private Transform closestEnemy;
    [SerializeField] private Animator animator;
    private float lastAttackTime;
    private bool isAttacking = false; // <-- Новый флаг

    void Start()
    {
        if (animator == null)
            Debug.LogWarning("Animator not assigned on " + gameObject.name, this);
    }

    void Update()
    {
        FindClosestEnemy();

        if (closestEnemy == null || !IsEnemyValid())
        {
            SetSpeed(0f);
            return;
        }

        if (isAttacking)
        {
            // Ничего не делаем — ждём завершения анимации
            return;
        }

        if (IsInAttackRange())
        {
            HandleAttack();
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
        if (Time.time - lastAttackTime < attackCooldown)
            return;

        // Запускаем атаку
        animator?.SetTrigger("Attack");
        lastAttackTime = Time.time;
        isAttacking = true; // Блокируем движение
    }

    void HandleMovement()
    {
        if (!IsEnemyValid()) 
        {
            SetSpeed(0f);
            return;
        }

        Vector3 targetPos = closestEnemy.position;
        Vector3 direction = targetPos - transform.position;
        direction.y = 0;
        float distance = direction.magnitude;

        if (distance > 0.01f)
        {
            if (rotateTowardsEnemy)
            {
                Quaternion targetRot = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }

            if (distance > stopDistance)
            {
                transform.position += direction.normalized * moveSpeed * Time.deltaTime;
                SetSpeed(moveSpeed);
            }
            else
            {
                SetSpeed(0f);
            }
        }
        else
        {
            SetSpeed(0f);
        }
    }

    public void ApplyDamage()
    {
        if (!IsEnemyValid()) return;

        IDamageable damageable = closestEnemy.GetComponent<IDamageable>();
        damageable?.TakeDamage(damage);
    }

    // ⚠️ Этот метод должен быть вызван через Animation Event в ПОСЛЕДНЕМ кадре анимации атаки
    public void OnAttackAnimationFinished()
    {
        isAttacking = false;
    }

    void SetSpeed(float speed)
    {
        animator?.SetFloat("Speed", speed);
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