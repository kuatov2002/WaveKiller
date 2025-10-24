using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class AttackEnemy : Action
{
    public SharedGameObject closestEnemy;
    public float damage = 10f;
    public float attackCooldown = 1f;
    public float attackDuration = 0.2f; // Время, в течение которого задача будет в состоянии Running после атаки

    private float lastAttackTime;
    private float attackStartTime;
    private Animator animator;
    private bool hasTriggered;
    private string _attackTrigger = "Attack";

    public override void OnStart()
    {
        animator = GetComponent<Animator>();
        hasTriggered = false;
    }

    public override TaskStatus OnUpdate()
    {
        if (closestEnemy.Value == null)
            return TaskStatus.Failure;

        // Проверяем, прошёл ли cooldown
        if (!hasTriggered && Time.time - lastAttackTime >= attackCooldown)
        {
            //animator.SetTrigger(_attackTrigger);
            lastAttackTime = Time.time;
            attackStartTime = Time.time;
            hasTriggered = true;
        }

        // Если атака уже запущена, ждём завершения задержки
        if (hasTriggered)
        {
            if (Time.time - attackStartTime >= attackDuration)
            {
                closestEnemy.Value.GetComponent<IDamageable>()?.TakeDamage(damage);
                return TaskStatus.Success; // Атака завершена
            }
            else
            {
                return TaskStatus.Running; // Ждём окончания анимации/задержки
            }
        }

        // На случай, если cooldown ещё не прошёл (редко, но возможно при частых вызовах)
        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        hasTriggered = false;
    }
}