using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class MoveToEnemy : Action
{
    public SharedGameObject closestEnemy;
    public float speed = 4f;

    private Transform myTransform;

    public override void OnStart()
    {
        myTransform = transform;
    }

    public override TaskStatus OnUpdate()
    {
        if (closestEnemy.Value == null)
            return TaskStatus.Failure;

        Vector3 direction = closestEnemy.Value.transform.position - myTransform.position;
        direction.y = 0; // если 3D и не хочешь подъёма по Y
        if (direction.magnitude < 0.1f)
            return TaskStatus.Success;

        myTransform.position += direction.normalized * speed * Time.deltaTime;
        return TaskStatus.Running;
    }
}