using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyAnimation : MonoBehaviour
{
    private static readonly string s_kMissingAnimatorText = "EnemyAnimation is missing an animator.";

    //The animator used by the object.
    private Animator m_enemyAnimator = null;

    // The parameter Identifier within the Animator component.
    private readonly int m_enemyMovingParameter = Animator.StringToHash("IsMoving");

    private void Start()
    {
        if (m_enemyAnimator == null)
            m_enemyAnimator = GetComponent<Animator>();
    }

    // TODO: Unfinished
    // Set the boolean within the animator that triggers the run/idle animations.
    public void SetPlayerMoving(bool isMoving)
    {
        if (m_enemyAnimator == null)
        {
            Debug.LogWarning(s_kMissingAnimatorText);
            return;
        }

        m_enemyAnimator.SetBool(m_enemyMovingParameter, isMoving);
    }
}
