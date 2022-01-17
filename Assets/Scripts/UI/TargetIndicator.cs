using UnityEngine;

public class TargetIndicator : MonoBehaviour
{
    private static readonly string s_kMissingPlayerText = "Indicator is missing a player controller.";

    // This indicator object.
    private GameObject m_thisObject = null;
    private Transform m_transform = null;

    [Tooltip("The player controller, this is used to verify the proper event response and only move the indicator when the player moves.")]
    [SerializeField] private PlayerController m_playerController = null;

    // The cached player object.
    private GameObject m_playerObject = null;

    private void Awake()
    {
        if (m_playerController == null)
        {
            Debug.LogError(s_kMissingPlayerText);
            return;
        }

        m_playerObject = m_playerController.gameObject;
        m_thisObject = gameObject;

        m_transform = transform;
        EventManager.AddListener(EventID.s_kCharacterMovedEvent, OnPlayerMoved);
        EventManager.AddListener(EventID.s_kCharacterStoppedEvent, OnPlayerStopped);

        // Disable this indicator to start.
        // I do this here because I need Awake to run.
        m_thisObject.SetActive(false);
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener(EventID.s_kCharacterMovedEvent, OnPlayerMoved);
        EventManager.RemoveListener(EventID.s_kCharacterStoppedEvent, OnPlayerStopped);
    }

    // Called when the player starts moving.
    // Displays and moves the indicator to the target.
    private void OnPlayerMoved(EventData data)
    {
        if (data.m_triggerObject == m_playerObject)
        {
            m_transform.position = data.m_targetPosition;
            m_thisObject.SetActive(true);
        }
    }

    // Called when the player stops moving.
    // Hides the indicator.
    private void OnPlayerStopped(EventData data)
    {
        if (data.m_triggerObject == m_playerObject)
        {
            m_thisObject.SetActive(false);
        }
    }
}
