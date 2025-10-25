using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float damage = 5f;
    private Vector3 direction;
    private float speed;
    private bool isInitialized = false;

    public void Initialize(Vector3 dir, float spd)
    {
        direction = dir.normalized;
        speed = spd;
        isInitialized = true;

        // Уничтожаем через 6 секунд
        Destroy(gameObject, 6f);
    }

    void Update()
    {
        if (!isInitialized) return;
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            IDamageable health = other.GetComponent<IDamageable>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }
    }
}