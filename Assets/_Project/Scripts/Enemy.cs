using System;
using Pathfinding;
using UnityEngine;

public class Enemy : MonoBehaviour
{
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
            Destroy(gameObject);
        }
    }
}
