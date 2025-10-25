using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class AttackEnemy : Action
{
    public SharedGameObject geometry;
    public SharedGameObject closestEnemy;
    public float damage = 10f;
    public float attackCooldown = 1.66f;

    public float attackDelay = 0.75f;
    public float postDelay = 0.91666f;

    private float lastAttackTime;
    private float phaseStartTime;
    private Animator animator;
    private bool hasTriggered;
    private int currentPhase; // 0 = idle, 1 = attack, 2 = post

    private const string _attackTrigger = "Attack";

    public override void OnStart()
    {
        animator = geometry.Value != null ? geometry.Value.GetComponent<Animator>() : null;
        if (animator == null)
        {
            Debug.LogWarning("Animator not found on 'geometry' object!");
        }

        hasTriggered = false;
        currentPhase = 0;
    }

    public override TaskStatus OnUpdate()
    {
        if (closestEnemy.Value == null)
            return TaskStatus.Failure;

        // Ждём окончания cooldown перед началом новой атаки
        if (currentPhase == 0 && Time.time - lastAttackTime >= attackCooldown)
        {
            // Сразу запускаем атаку (без pre-delay)
            animator?.SetTrigger(_attackTrigger);
            hasTriggered = true;
            currentPhase = 1;
            phaseStartTime = Time.time;
        }

        // Фаза 1: Ожидание основной атаки (attackDelay), затем наносим урон
        if (currentPhase == 1)
        {
            if (Time.time - phaseStartTime >= attackDelay)
            {
                closestEnemy.Value.GetComponent<IDamageable>()?.TakeDamage(damage);
                currentPhase = 2;
                phaseStartTime = Time.time;
            }
            return TaskStatus.Running;
        }

        // Фаза 2: Восстановление (post-delay)
        if (currentPhase == 2)
        {
            if (Time.time - phaseStartTime >= postDelay)
            {
                lastAttackTime = Time.time;
                return TaskStatus.Success;
            }
        }

        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        // Сброс состояния при прерывании
        hasTriggered = false;
        currentPhase = 0;
    }
}