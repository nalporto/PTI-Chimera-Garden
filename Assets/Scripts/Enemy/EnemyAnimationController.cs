using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void PlayAttack()
    {
        if (animator != null)
            animator.SetTrigger("Attack");
    }
}