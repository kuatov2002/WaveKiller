using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class IsInAttackRange : Conditional
{
    public SharedGameObject closestEnemy;
    public float attackRange = 2f;

    public override TaskStatus OnUpdate()
    {
        if (closestEnemy.Value == null) return TaskStatus.Failure;
        float dist = Vector3.Distance(transform.position, closestEnemy.Value.transform.position);
        return dist <= attackRange ? TaskStatus.Success : TaskStatus.Failure;
    }
}