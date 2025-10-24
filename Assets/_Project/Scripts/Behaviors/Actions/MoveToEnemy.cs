using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class MoveToEnemy : Action
{
    public SharedGameObject closestEnemy;
    public float speed = 4f;
    public float detectionRadius = 3f;
    public float facingAngleThreshold = 10f; // насколько точно нужно смотреть на врага (в градусах)
    public bool rotateTowardsEnemy = true;

    private Transform myTransform;

    public override void OnStart()
    {
        myTransform = transform;
    }

    public override TaskStatus OnUpdate()
    {
        if (closestEnemy.Value == null)
            return TaskStatus.Failure;

        Vector3 targetPosition = closestEnemy.Value.transform.position;
        Vector3 direction = targetPosition - myTransform.position;
        direction.y = 0f;

        float distance = direction.magnitude;

        // Поворот к врагу (всегда, если включено)
        if (rotateTowardsEnemy && distance > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        // Если далеко — двигаемся
        if (distance > detectionRadius)
        {
            myTransform.position += direction.normalized * speed * Time.deltaTime;
            return TaskStatus.Running;
        }

        // Если рядом — проверяем, смотрим ли мы на врага
        if (distance <= detectionRadius)
        {
            float angle = Vector3.Angle(myTransform.forward, direction);
            if (angle <= facingAngleThreshold)
            {
                return TaskStatus.Success; // рядом и смотрим
            }
        }

        return TaskStatus.Running;
    }
}