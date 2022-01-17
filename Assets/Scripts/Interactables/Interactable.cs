using UnityEngine;

[System.Serializable]
public class Interactable : MonoBehaviour
{
    // Interaction Range
    public float interactRange
    {
        get { return m_interactRange; }
    }

    [Tooltip("The distance required in order to begin an interaction with this Interactable.")]
    [SerializeField] private float m_interactRange = 2.0f;

    // Whether or not the player has already interacted with this interactable.
    // This is reset when the target is retargeted.
    private bool m_hasInteracted = false;

    // Cached Interactable transform.
    // NOTE: For some reason, this doesn't work and m_transform becomes null,
    // I think this is due to this being a base class?
    //private Transform m_transform = null;
    //private void Awake() { m_transform = transform; }

    // Used in editor to display the interaction radius.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        // SEE NOTE ABOVE
        //if (m_transform == null)
        //    m_transform = transform;
        //Gizmos.DrawWireSphere(m_transform.position, m_interactRange);

        Gizmos.DrawWireSphere(transform.position, m_interactRange);
    }

    // Override this function for interaction behavior.
    protected virtual void Interact(GameObject interactor) { }

    // This is called if this interactable is targeted by the player
    // during Update().
    public void OnInteract(GameObject interactor)
    {
        //if (!m_hasInteracted && m_transform != null && interactor != null)
        if (m_hasInteracted || transform == null || interactor == null)
            return;

        // SEE NOTE ABOVE
        //float distance = Vector3.Distance(interactor.transform.position, m_transform.position);
        float distance = Vector3.Distance(interactor.transform.position, transform.position);

        if (distance <= m_interactRange)
        {
            Interact(interactor);
            m_hasInteracted = true;
        }
    }

    // Allows this object to be interacted with again.
    public void ResetInteractionState()
    {
        m_hasInteracted = false;
    }
}
