using MoreMountains.Feedbacks;
using UnityEngine;
using Pathfinding;
using System.Collections;

public class MainAIController : MonoBehaviour
{
    [Header("Feedbacks")]
    public MMFeedbacks hitFeedback;
    
    // === Настройки ===
    [Header("Detection")]
    public float detectionRadius = 30f;

    [Header("Movement")]
    public float moveSpeed = 4f;
    public float stopDistance = 2f;
    public bool rotateTowardsEnemy = true;

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
    private bool isRotatingToEnemy = false; // ← НОВОЕ

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
        // Не трогаем rotationSpeed здесь — будем управлять вручную при атаке
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

        if (isAttacking || isRotatingToEnemy)
            return;

        if (IsInAttackRange())
        {
            HandleAttack();
            SetDestination(transform);
        }
        else
        {
            HandleMovement();
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
        richAI.canMove = false;
        isAttacking = true;
        isRotatingToEnemy = true;

        StartCoroutine(RotateTowardsEnemySmoothly());
    }

    IEnumerator RotateTowardsEnemySmoothly()
    {
        if (!IsEnemyValid())
        {
            CancelRotation();
            yield break;
        }

        Vector3 targetDirection = closestEnemy.position - transform.position;
        targetDirection.y = 0;
        if (targetDirection.sqrMagnitude < 0.01f)
        {
            // Враг слишком близко — сразу атакуем
            FinishRotationAndAttack();
            yield break;
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        // Используем rotationSpeed из RichAI, если rotateTowardsEnemy == true, иначе задаём своё
        float rotationSpeed = rotateTowardsEnemy && richAI.rotationSpeed > 0 
            ? richAI.rotationSpeed 
            : 360f; // градусов в секунду

        float angleDiff = Quaternion.Angle(transform.rotation, targetRotation);
        float duration = angleDiff / rotationSpeed;

        Quaternion startRotation = transform.rotation;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;
        FinishRotationAndAttack();
    }

    void FinishRotationAndAttack()
    {
        isRotatingToEnemy = false;
        animator?.SetTrigger("Attack");
        lastAttackTime = Time.time;
    }

    void CancelRotation()
    {
        isRotatingToEnemy = false;
        isAttacking = false;
        richAI.canMove = true;
    }

    // ... остальные методы без изменений ...

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

    public void ApplyDamage()
    {
        if (!IsEnemyValid()) return;

        IDamageable damageable = closestEnemy.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
            hitFeedback?.PlayFeedbacks();
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