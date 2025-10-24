using UnityEngine;

public class MainCharacter : MonoBehaviour, IDamageable
{
    public float maxHp = 100f;
    public float currentHp = 100f;

    private void Start()
    {
        currentHp = maxHp;
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
        Debug.Log("я умер");
    }
}
