using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class FindClosestEnemy : Action
{
    public SharedGameObject closestEnemy;
    public SharedFloat detectionRadius = 20f;

    public override TaskStatus OnUpdate()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0)
        {
            closestEnemy.Value = null;
            return TaskStatus.Failure;
        }

        GameObject closest = null;
        float minDist = Mathf.Infinity;
        Vector3 myPos = transform.position;

        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;
            float dist = Vector3.Distance(myPos, enemy.transform.position);
            if (dist <= detectionRadius.Value && dist < minDist)
            {
                minDist = dist;
                closest = enemy;
            }
        }

        closestEnemy.Value = closest;
        return closest != null ? TaskStatus.Success : TaskStatus.Failure;
    }
}