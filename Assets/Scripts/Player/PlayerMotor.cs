using System.Collections;
using UnityEngine.AI;
using UnityEngine;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMotor : MonoBehaviour
{
    // This objects owned navmeshagent component.
    private NavMeshAgent m_agentComponent = null;

    // The currently executing coroutine.
    private IEnumerator m_currentCoroutine = null;

    // Cached Event Data
    private EventData m_eventData = new EventData();

    void Awake()
    {
        if (m_agentComponent == null)
            m_agentComponent = GetComponent<NavMeshAgent>();

        m_eventData.m_triggerObject = this.gameObject;
    }

    // Returns true if the nav mesh agent is not currently pathing.
    public bool IsStopped()
    {
        bool isCloseEnough = false;
        if (m_agentComponent.remainingDistance <= m_agentComponent.stoppingDistance)
            isCloseEnough = true;

        return !m_agentComponent.hasPath || isCloseEnough;
    }

    // Attempts to move the players nav mesh agent to the given point.
    public void MoveToPoint(Vector3 targetPoint)
    {
        CancelAction();
        m_currentCoroutine = _MoveToPoint(targetPoint);

        TriggerPlayerMovedEvent(targetPoint);
        StartCoroutine(m_currentCoroutine);
    }

    // Moves to a target object, even if it is moving.
    public void MoveToTarget(Interactable target)
    {
        CancelAction();
        m_currentCoroutine = _MoveToTarget(target);

        TriggerPlayerMovedEvent(target.transform.position);
        StartCoroutine(m_currentCoroutine);
    }

    // Cancels the current coroutine, if any.
    public void CancelAction()
    {
        if (m_currentCoroutine != null)
        {
            StopCoroutine(m_currentCoroutine);
            m_currentCoroutine = null;
        }
    }

    // Moves the player to the dynamic target gameobject.
    private IEnumerator _MoveToTarget(Interactable target)
    {
        Transform targetTransform = target.transform;

        if (targetTransform == null)
        {
            TriggerPlayerStoppedEvent();
            yield break;
        }

        if (m_agentComponent != null)
            m_agentComponent.stoppingDistance = target.interactRange;

        do
        {
            if (targetTransform == null)
                break;

            if (m_agentComponent != null)
                m_agentComponent.SetDestination(targetTransform.position);
            TriggerPlayerMovedEvent(targetTransform.position);
            yield return null;
        }
        while (!IsStopped());

        TriggerPlayerStoppedEvent();

        // We clear our path so that we can reset our stopping distance.
        if (m_agentComponent != null)
        {
            m_agentComponent.ResetPath();
            m_agentComponent.stoppingDistance = 0.0f;
        }
    }

    // Move the player to the static world point.
    private IEnumerator _MoveToPoint(Vector3 target)
    {
        do
        {
            if (m_agentComponent != null)
                m_agentComponent.SetDestination(target);
            yield return null;
        }
        while (!IsStopped()) ;

        TriggerPlayerStoppedEvent();
    }

    // Helpers for triggering events.
    private void TriggerPlayerMovedEvent(Vector3 newPosition)
    {
        m_eventData.m_targetPosition = newPosition;
        EventManager.TriggerEvent(EventID.s_kCharacterMovedEvent, m_eventData);
    }

    // Helpers for triggering events.
    private void TriggerPlayerStoppedEvent()
    {
        EventManager.TriggerEvent(EventID.s_kCharacterStoppedEvent, m_eventData);
    }
}
