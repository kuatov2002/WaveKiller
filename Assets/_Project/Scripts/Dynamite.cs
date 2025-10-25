using System;
using UnityEngine;

public class Dynamite : MonoBehaviour
{
    [SerializeField] private ParticleSystem explosionVFX;
    public float radius = 2f; // Значение по умолчанию для удобства в редакторе

    private Collider[] colliders = new Collider[20];

    private void OnCollisionEnter(Collision other)
    {
        Invoke(nameof(PlayExplosionVFX), 1f);
    }

    private void PlayExplosionVFX()
    {
        if (explosionVFX != null)
        {
            Instantiate(explosionVFX, transform.position, Quaternion.identity);

            int count = Physics.OverlapSphereNonAlloc(transform.position, radius, colliders);

            for (int i = 0; i < count; i++)
            {
                Collider col = colliders[i];

                IDamageable damageable = col.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(150f);
                }
            }
            
            Destroy(gameObject);
        }
    }
#if UNITY_EDITOR
    // Рисуем гизму радиуса взрыва в редакторе
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
#endif
}