using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float damage = 5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IDamageable health = other.GetComponent<IDamageable>();
            if (health != null)
                health.TakeDamage(damage);
        }
        // Уничтожить стрелу после попадания
        Destroy(gameObject);
    }
}