using Pathfinding;
using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour, IDamageable
{
    public float currentHp = 10f;
    public float damage = 10f;
    public float attackRadius = 2f;
    public float attackAngle = 90f; // полный угол обзора в градусах
    public float attackCooldown = 3f;
    public float attackDelay = 0.5f;

    private AIDestinationSetter destinationSetter;
    private Transform player;
    private float lastAttackTime;
    private bool isAttacking = false;

    private void Start()
    {
        destinationSetter = GetComponent<AIDestinationSetter>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (destinationSetter != null && player != null)
        {
            destinationSetter.target = player;
        }
    }

    private void Update()
    {
        if (player == null || isAttacking) return;

        if (CanSeePlayer() && Vector3.Distance(transform.position, player.position) <= attackRadius)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                StartCoroutine(AttackWithDelay());
            }
        }
    }

    private bool CanSeePlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        return angle <= attackAngle * 0.5f;
    }

    private IEnumerator AttackWithDelay()
    {
        isAttacking = true;
        yield return new WaitForSeconds(attackDelay);
        Attack();
        lastAttackTime = Time.time;
        isAttacking = false;
    }

    public void Attack()
    {
        IDamageable playerDamageable = player.GetComponent<IDamageable>();
        if (playerDamageable != null)
        {
            playerDamageable.TakeDamage(damage);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHp -= damage;
        if (currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

#if UNITY_EDITOR
    // === Gizmos для визуализации зоны атаки ===
    private void OnDrawGizmos()
    {
        // Радиус атаки — окружность на плоскости XZ (игровая плоскость)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        // Конус атаки (угол)
        Gizmos.color = Color.yellow;

        float halfAngle = attackAngle * 0.5f;
        Vector3 forward = transform.forward;

        // Левая и правая границы конуса
        Vector3 leftDir = Quaternion.Euler(0, -halfAngle, 0) * forward;
        Vector3 rightDir = Quaternion.Euler(0, halfAngle, 0) * forward;

        // Длина линий — attackRadius
        Gizmos.DrawLine(transform.position, transform.position + leftDir * attackRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightDir * attackRadius);

        // Дополнительно: дуга между границами (для лучшей визуализации)
        int segments = 20;
        float angleStep = attackAngle / segments;
        Vector3 lastPoint = transform.position + leftDir * attackRadius;

        for (int i = 1; i <= segments; i++)
        {
            float angle = -halfAngle + angleStep * i;
            Vector3 dir = Quaternion.Euler(0, angle, 0) * forward;
            Vector3 point = transform.position + dir * attackRadius;
            Gizmos.DrawLine(lastPoint, point);
            lastPoint = point;
        }
    }
#endif
}