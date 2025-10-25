using UnityEngine;

public class AiAnimationEventsMage : MonoBehaviour
{
    [SerializeField] private MageAIController mainAIController;
    public void ShootFireball()
    {
        mainAIController.ShootFireball();
    }

    public void OnAttackAnimationFinished()
    {
        mainAIController.OnAttackAnimationFinished();
    }
}
