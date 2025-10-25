using UnityEngine;

public class AiAnimationEvents : MonoBehaviour
{
    [SerializeField] private MainAIController mainAIController;
    public void ApplyDamage()
    {
        mainAIController.ApplyDamage();
    }

    public void OnAttackAnimationFinished()
    {
        mainAIController.OnAttackAnimationFinished();
    }
}
