using UnityEngine;
using Pathfinding;

public class ArcherAI : MonoBehaviour, IDamageable
{
    [Header("Targeting")]
    public float maxAttackRange = 20f;
    public float stopDistance = 15f;

    [Header("Shooting")]
    public float damage = 5f;
    public float fireRate = 2f;
    public float arrowSpeed = 30f; // Скорость стрелы

    [Header("References")]
    public GameObject arrowPrefab; // Префаб стрелы
    public Transform shootPoint;   // Точка выстрела (пустой объект в руках/луке)

    private Transform player;
    private RichAI richAI;
    private AIDestinationSetter destinationSetter;
    private float lastShotTime;

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
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= maxAttackRange)
        {
            // Повернуть арчера к игроку (опционально, если нужно)
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

            if (Time.time - lastShotTime >= fireRate)
            {
                Shoot();
                lastShotTime = Time.time;
            }

            richAI.canMove = false;
        }
        else
        {
            richAI.canMove = true;
        }
    }

    void Shoot()
    {
        Debug.Log("Archer shoots!");

        if (arrowPrefab == null || shootPoint == null || player == null) return;

        // Горизонтальное расстояние (игнорируем высоту)
        Vector3 toPlayer = player.position - shootPoint.position;
        float horizontalDistance = new Vector3(toPlayer.x, 0, toPlayer.z).magnitude;

        // Направление в горизонтальной плоскости (без Y)
        Vector3 flatDirection = new Vector3(toPlayer.x, 0, toPlayer.z).normalized;

        // Чем дальше — тем выше подъём (линейно или по кривой)
        // Настрой параметры под свои нужды
        float upFactor = Mathf.Clamp(horizontalDistance * 0.2f, 5f, 20f); // Пример: от 5 до 20

        // Итоговая начальная скорость
        Vector3 launchVelocity = flatDirection * arrowSpeed + Vector3.up * upFactor;

        // Создаём стрелу
        GameObject arrowInstance = Instantiate(arrowPrefab, shootPoint.position, Quaternion.LookRotation(launchVelocity));
        Rigidbody rb = arrowInstance.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = launchVelocity;
        }
        else
        {
            Debug.LogError("Arrow prefab must have a Rigidbody!");
        }

        // Передаём урон
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
