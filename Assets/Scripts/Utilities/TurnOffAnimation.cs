using UnityEngine;

public class TurnOffAnimation : MonoBehaviour
{
    [SerializeField] Animator animator;
    internal void TurnOffAnimator()
    {
        animator.enabled = false;
    }
}
