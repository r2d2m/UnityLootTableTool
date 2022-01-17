using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimation : MonoBehaviour
{
    private static readonly string s_kMissingAnimatorText = "PlayerAnimation is missing an animator.";

    // The animator used by the object.
    private Animator m_playerAnimator = null;

    // The cached gameobject for this animation script.
    private GameObject m_thisObject = null;

    // The parameter Identifiers within the Animator component.
    private static readonly int m_playerMovingParameter = Animator.StringToHash("IsMoving");
    private static readonly int m_playerAttackingParameter = Animator.StringToHash("Attack");

    private void Start()
    {
        if (m_playerAnimator == null)
            m_playerAnimator = GetComponent<Animator>();

        m_thisObject = this.gameObject;

        // Listen to expected events.
        EventManager.AddListener(EventID.s_kCharacterMovedEvent, OnPlayerMoved);
        EventManager.AddListener(EventID.s_kCharacterStoppedEvent, OnPlayerStopped);
        EventManager.AddListener(EventID.s_kCharacterAttackedEvent, OnPlayerAttack);
    }

    private void OnDestroy()
    {
        // Stop listening.
        EventManager.RemoveListener(EventID.s_kCharacterMovedEvent, OnPlayerMoved);
        EventManager.RemoveListener(EventID.s_kCharacterStoppedEvent, OnPlayerStopped);
        EventManager.RemoveListener(EventID.s_kCharacterAttackedEvent, OnPlayerAttack);
    }

    // Set the boolean within the animator that triggers the run/idle animations.
    private void SetPlayerMoving(bool isMoving)
    {
        if (m_playerAnimator == null)
        {
            Debug.LogWarning(s_kMissingAnimatorText);
            return;
        }

        m_playerAnimator.SetBool(m_playerMovingParameter, isMoving);
    }

    // Set the trigger within the animator that activates the attack animation.
    private void SetPlayerAttack()
    {
        if (m_playerAnimator == null)
        {
            Debug.LogWarning(s_kMissingAnimatorText);
            return;
        }

        m_playerAnimator.SetTrigger(m_playerAttackingParameter);
    }

    // Callback for when the player starts moving.
    private void OnPlayerMoved(EventData data)
    {
        if (data.m_triggerObject == m_thisObject)
            SetPlayerMoving(true);
    }

    // Callback for when the player stops moving.
    private void OnPlayerStopped(EventData data)
    {
        if (data.m_triggerObject == m_thisObject)
            SetPlayerMoving(false);
    }

    // Callback for when the player attacks.
    private void OnPlayerAttack(EventData data)
    {
        if (data.m_triggerObject == m_thisObject)
            SetPlayerAttack();
    }
}
