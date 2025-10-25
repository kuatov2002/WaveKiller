using UnityEngine;
using Pathfinding;
using System.Collections;

public class ArcherAI : MonoBehaviour, IDamageable
{
    [Header("Targeting")]
    public float maxAttackRange = 20f;
    public float stopDistance = 15f;

    [Header("Shooting")]
    public float damage = 5f;
    public float fireRate = 2f;
    public float arrowSpeed = 30f;
    public float attackDelay = 1f; // Задержка между началом анимации атаки и выпуском стрелы

    [Header("References")]
    public GameObject arrowPrefab;
    public Transform shootPoint;
    public Animator animator; // Ссылка на Animator

    private Transform player;
    private RichAI richAI;
    private AIDestinationSetter destinationSetter;
    private float lastShotTime;
    private bool isAttacking = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogWarning("Player not found!");

        richAI = GetComponent<RichAI>();
        if (richAI == null)
            Debug.LogError("RichAI component missing!");

        destinationSetter = GetComponent<AIDestinationSetter>();
        if (destinationSetter != null)
            destinationSetter.target = player;

        if (arrowPrefab == null)
            Debug.LogError("Arrow prefab is not assigned!");

        if (shootPoint == null)
            Debug.LogError("Shoot point is not assigned!");

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (player == null || isAttacking) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Обновляем параметр скорости для аниматора
        float currentSpeed = richAI.desiredVelocity.magnitude;
        animator?.SetFloat("Speed", currentSpeed);

        if (distanceToPlayer <= maxAttackRange)
        {
            // Повернуть к игроку (игнорируя Y)
            Vector3 lookPos = new Vector3(player.position.x, transform.position.y, player.position.z);
            transform.LookAt(lookPos);

            if (Time.time - lastShotTime >= fireRate)
            {
                StartCoroutine(AttackRoutine());
            }

            richAI.canMove = false;
        }
        else
        {
            richAI.canMove = true;
        }
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        lastShotTime = Time.time;

        // Запускаем анимацию атаки
        animator?.SetTrigger("Attack");

        // Ждём заданную задержку перед выпуском стрелы
        yield return new WaitForSeconds(attackDelay);

        Shoot();

        isAttacking = false;
    }

    void Shoot()
    {
        Debug.Log("Archer shoots!");

        if (arrowPrefab == null || shootPoint == null || player == null) return;

        Vector3 toPlayer = player.position - shootPoint.position;
        Vector3 flatDirection = new Vector3(toPlayer.x, 0, toPlayer.z).normalized;
        float horizontalDistance = flatDirection.magnitude == 0 ? toPlayer.magnitude : flatDirection.magnitude * toPlayer.magnitude;

        // Настройка траектории
        float upFactor = Mathf.Clamp(horizontalDistance * 0.2f, 5f, 20f);
        Vector3 launchVelocity = flatDirection * arrowSpeed + Vector3.up * upFactor;

        GameObject arrowInstance = Instantiate(arrowPrefab, shootPoint.position, Quaternion.LookRotation(launchVelocity));
        Rigidbody rb = arrowInstance.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = launchVelocity; // Используем .velocity вместо linearVelocity (Unity стандарт)
        }
        else
        {
            Debug.LogError("Arrow prefab must have a Rigidbody!");
        }

        Arrow arrowScript = arrowInstance.GetComponent<Arrow>();
        if (arrowScript != null)
        {
            arrowScript.damage = damage;
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Color original = Gizmos.color;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxAttackRange);

        if (stopDistance > 0 && stopDistance < maxAttackRange)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, stopDistance);
        }

        Gizmos.color = original;
    }
#endif

    public void TakeDamage(float damage)
    {
        Die();
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}