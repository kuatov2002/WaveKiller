using Pathfinding;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public float currentHp = 10f;
    public float damage = 10f;
    public float attackRadius = 2f;
    public float attackAngle = 90f; // угол в градусах (attackCorner)
    public float attackCooldown = 1f; // секунды между атаками

    private AIDestinationSetter destinationSetter;
    private Transform player;
    private float lastAttackTime;

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
        if (player == null) return;

        /*// Проверяем, находится ли игрок в зоне атаки
        if (CanSeePlayer() && Vector3.Distance(transform.position, player.position) <= attackRadius)
        {
            // Поворачиваем врага к игроку (опционально)
            transform.LookAt(player);

            // Атакуем с учётом cooldown
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                //Attack();
                lastAttackTime = Time.time;
            }
        }*/
    }

    /*private bool CanSeePlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        return angle <= attackAngle * 0.5f; // attackAngle — полный угол, поэтому делим пополам
    }*/

    /*public void Attack()
    {
        // Попытка нанести урон игроку
        IDamageable playerDamageable = player.GetComponent<IDamageable>();
        if (playerDamageable != null)
        {
            playerDamageable.TakeDamage(damage);
        }
    }*/

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
}