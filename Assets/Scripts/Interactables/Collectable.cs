using UnityEngine;

[System.Serializable]
public class Collectable : Interactable
{
    // The item held by this collectable.
    public Item item { get { return m_item; } set { value = m_item; } }

    [Tooltip("The item to be added to the players inventory when collected.")]
    [SerializeField] private Item m_item = null;

    // This collectables cached game object.
    private GameObject m_thisObject = null;

    // The event that this collectable will fire on collect.
    private EventData m_collectEventData = new EventData();

    private void Awake()
    {
        m_thisObject = this.gameObject;
        m_collectEventData.m_targetObject = m_thisObject;
    }

    // Called when the player interacts with this collectable.
    protected override void Interact(GameObject interactor)
    {
        TriggerCollectedItemEvent(interactor);
    }

    // Helper for triggering the Item collection event.
    private void TriggerCollectedItemEvent(GameObject interactor)
    {
        if (interactor != null)
            m_collectEventData.m_triggerObject = interactor;
        else
            m_collectEventData.m_triggerObject = null;

        EventManager.TriggerEvent(EventID.s_kItemCollectionEvent, m_collectEventData);
    }
}
