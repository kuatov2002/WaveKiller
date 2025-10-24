using Pathfinding;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public float currentHp = 10f;
    public float damage = 10f;
    private AIDestinationSetter destinationSetter;
    private void Start()
    {
        destinationSetter = GetComponent<AIDestinationSetter>();
        destinationSetter.target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<IDamageable>()?.TakeDamage(damage);
            Die();
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
}
