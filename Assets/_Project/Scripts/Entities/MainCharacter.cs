using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainCharacter : MonoBehaviour, IDamageable
{
    public float maxHp = 100f;
    public float currentHp = 100f;

    public Slider hp;
    
    private bool isDead = false;
    private void Start()
    {
        currentHp = maxHp;
        UpdateHpBar();
    }
    
    public void TakeDamage(float damage)
    {
        currentHp -= damage;
        UpdateHpBar();
        if (currentHp <= 0)
        {
            Die();
        }
    }

    private void UpdateHpBar()
    {
        hp.value = currentHp / maxHp;
    }
    
    private void Die()
    {
        if (isDead || StaticEvents.IsGameOver) return;
        isDead = true;
        StaticEvents.OnGameOver.Invoke(true);
        StaticEvents.IsGameOver = true;
        
        Destroy(gameObject, 0.2f);
    }
}
