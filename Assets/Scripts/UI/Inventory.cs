using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour
{
    private static readonly string s_kNullGameobjectText = "Unable to add item, GameObject was null.";
    private static readonly string s_kFullInventoryText = "Inventory is full.";

    // The list of items inside the players inventory currently.
    private List<Item> m_items = new List<Item>();

    [Tooltip("The maximum numbr of slots in the inventory.")]
    [SerializeField] private int m_maxInventorySlots = 0;

    private InventorySlot[] m_slots = null;

    [Tooltip("The prefab for the child item slots.")]
    [SerializeField] private GameObject m_itemSlotPrefab = null;

    // the cached gameobject.
    private GameObject m_thisObject = null;

    // The event that this inventory will fire on add or remove.
    private EventData m_inventoryEventData = new EventData();

    private void Awake()
    {
        EventManager.AddListener(EventID.s_kItemCollectionEvent, AddItem);
        EventManager.AddListener(EventID.s_kInventorySlotCleared, RemoveItem);
        EventManager.AddListener(EventID.s_kToggleInventoryUIEvent, ToggleInventory);

        m_thisObject = this.gameObject;
        m_inventoryEventData.m_triggerObject = m_thisObject;

        CreateChildInventorySlots();

        m_thisObject.SetActive(false);
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener(EventID.s_kItemCollectionEvent, AddItem);
        EventManager.RemoveListener(EventID.s_kInventorySlotCleared, RemoveItem);
        EventManager.RemoveListener(EventID.s_kToggleInventoryUIEvent, ToggleInventory);
    }

    // Create the number of slots given by m_maxslots.
    private void CreateChildInventorySlots()
    {
        for (int i = 0; i < m_maxInventorySlots; ++i)
        {
            GameObject.Instantiate(m_itemSlotPrefab, transform);
        }

        m_slots = m_thisObject.GetComponentsInChildren<InventorySlot>();
    }

    // Hide or Display the inventory, callback when player presses inventory key.
    private void ToggleInventory(EventData data)
    {
        // Toggle Inventory if not in UI menu.
        if (!EventSystem.current.IsPointerOverGameObject())
            m_thisObject.SetActive(!m_thisObject.activeSelf);
    }

    // Update the item slots after an update.
    private void UpdateInventorySlots()
    {
        for (int i = 0; i < m_slots.Length; ++i)
        {
            if (i < m_items.Count)
                m_slots[i].SetItem(m_items[i]);
            else
                m_slots[i].RemoveItem();
        }
    }

    // Called when the player interacts with any collectable.
    private void AddItem(EventData data)
    {
        if (data.m_targetObject == null)
        {
            Debug.LogError(s_kNullGameobjectText);
            return;
        }

        if (m_items.Count < m_maxInventorySlots)
        {
            Collectable collectable = data.m_targetObject.GetComponent<Collectable>();
            if (collectable != null)
                m_items.Add(collectable.item);

            UpdateInventorySlots();

            GameObject.Destroy(data.m_targetObject);
        }
        else
            Debug.Log(s_kFullInventoryText);
    }

    // Called when the player removes any inventory item.
    private void RemoveItem(EventData data)
    {
        if (data.m_targetObject == null)
        {
            Debug.LogError(s_kNullGameobjectText);
            return;
        }

        Collectable collectable = data.m_targetObject.GetComponent<Collectable>();
        if (collectable != null && m_items.Contains(collectable.item))
        {
            m_items.Remove(collectable.item);

            UpdateInventorySlots();
        }
    }
}
