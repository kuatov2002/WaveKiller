using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class AttackEnemy : Action
{
    public SharedGameObject closestEnemy;
    public float damage = 10f;
    public float attackCooldown = 1f;

    private float lastAttackTime;

    public override TaskStatus OnUpdate()
    {
        if (closestEnemy.Value == null) return TaskStatus.Failure;

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            // Наносим урон (просто уничтожаем для геймджема)
            Debug.Log("ударил");
            lastAttackTime = Time.time;
        }

        return TaskStatus.Success; // или Running, если хочешь анимацию
    }
}