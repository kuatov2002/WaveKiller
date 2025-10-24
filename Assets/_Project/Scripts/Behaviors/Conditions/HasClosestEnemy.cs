using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class HasClosestEnemy : Conditional
{
    public SharedGameObject closestEnemy;

    public override TaskStatus OnUpdate()
    {
        return closestEnemy.Value != null ? TaskStatus.Success : TaskStatus.Failure;
    }
}