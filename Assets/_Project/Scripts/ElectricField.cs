using UnityEngine;

public class ElectricField : MonoBehaviour
{
    public float radius = 3f;
    public float damagePerSecond = 5f; // урон в секунду

    private Collider[] colliders = new Collider[20]; // буфер для избежания аллокаций

    private void Update()
    {
        // Количество урона за этот кадр
        float damageThisFrame = damagePerSecond * Time.deltaTime;

        // Получаем все коллайдеры в радиусе (без аллокации памяти)
        int count = Physics.OverlapSphereNonAlloc(transform.position, radius, colliders);

        for (int i = 0; i < count; i++)
        {
            Collider col = colliders[i];

            // Игнорируем, если это не враг (или не тот тег)
            if (!col.CompareTag("Enemy"))
                continue;

            // Пытаемся получить интерфейс IDamageable
            IDamageable damageable = col.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damageThisFrame);
            }
        }
    }

#if UNITY_EDITOR
    // Для визуализации в редакторе
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
#endif
}