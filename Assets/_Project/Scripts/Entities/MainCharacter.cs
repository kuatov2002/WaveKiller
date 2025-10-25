using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainCharacter : MonoBehaviour, IDamageable
{
    public float maxHp = 100f;
    public float currentHp = 100f;

    public Image hp;
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
        hp.fillAmount = currentHp / maxHp;
    }
    
    private void Die()
    {
        Debug.Log("я умер");
    }
}
