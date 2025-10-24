using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class IsInAttackRange : Conditional
{
    public SharedGameObject closestEnemy;
    public float attackRange = 2f;
    public float attackAngle = 90f; // полный угол обзора (например, 90° = ±45° от forward)

    public override TaskStatus OnUpdate()
    {
        if (closestEnemy.Value == null)
            return TaskStatus.Failure;

        Transform enemyTransform = closestEnemy.Value.transform;
        Vector3 toEnemy = enemyTransform.position - transform.position;
        toEnemy.y = 0; // игнорируем высоту

        // Проверка расстояния
        if (toEnemy.magnitude > attackRange)
            return TaskStatus.Failure;

        // Проверка угла
        float angle = Vector3.Angle(transform.forward, toEnemy);
        if (angle <= attackAngle * 0.5f) // потому что attackAngle — полный угол
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}