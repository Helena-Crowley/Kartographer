using UnityEngine;

public class WendyAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public void PlayIdle()
    {
        animator.SetTrigger("Idle");
    }

    public void PlayAlert()
    {
        animator.SetTrigger("Alert");
    }

    public void PlayScream()
    {
        animator.SetTrigger("Scream");
    }

    public void PlayChase()
    {
        animator.SetTrigger("Chase");
    }

    public void PlayDespawn()
    {
        animator.SetTrigger("Despawn");
    }

    public void PlayAttack()
    {
        animator.SetTrigger("Attack");
    }
}
