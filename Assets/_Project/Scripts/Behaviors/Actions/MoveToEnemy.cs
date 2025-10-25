using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class MoveToEnemy : Action
{
    public SharedGameObject geometry;
    public SharedGameObject closestEnemy;
    public float speed = 4f;
    public float detectionRadius = 3f;
    public float facingAngleThreshold = 10f;
    public bool rotateTowardsEnemy = true;

    private Transform myTransform;
    private Animator animator;

    public override void OnStart()
    {
        myTransform = transform;
        animator = geometry.Value != null ? geometry.Value.GetComponent<Animator>() : null;

        if (animator == null)
        {
            Debug.LogWarning("Animator not found on 'geometry' object!");
        }
    }

    public override TaskStatus OnUpdate()
    {
        float currentSpeed = 0f; // по умолчанию стоим

        if (closestEnemy.Value == null)
        {
            // Нет цели → стоим
            if (animator != null)
                animator.SetFloat("Speed", 0f);
            return TaskStatus.Failure;
        }

        Vector3 targetPosition = closestEnemy.Value.transform.position;
        Vector3 direction = targetPosition - myTransform.position;
        direction.y = 0f;
        float distance = direction.magnitude;

        // Поворот к врагу
        if (rotateTowardsEnemy && distance > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        // Движение и скорость
        if (distance > detectionRadius)
        {
            // Двигаемся к врагу
            myTransform.position += direction.normalized * speed * Time.deltaTime;
            currentSpeed = speed; // полная скорость
        }
        else
        {
            // В зоне → не двигаемся
            currentSpeed = 0f;
        }

        // Передаём скорость в Animator
        if (animator != null)
        {
            animator.SetFloat("Speed", currentSpeed);
        }

        // Логика завершения
        if (distance <= detectionRadius)
        {
            float angle = Vector3.Angle(myTransform.forward, direction);
            if (angle <= facingAngleThreshold)
            {
                return TaskStatus.Success;
            }
        }

        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        // На всякий случай обнуляем скорость при завершении
        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
        }
    }
}