using UnityEngine.UI;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    [Tooltip("The sprite used for the default inventory slot.")]
    [SerializeField] private Sprite m_defaultInventorySlotSprite = null;

    // Cached "this" as image.
    private Image m_thisImage = null;

    // public accessor for the item in this slot.
    public Item item { get; }

    // The current item in the item slot.
    // The image we use is extracted from here.
    private Item m_slotCurrentItem = null;

    private void Awake()
    {
        if (m_thisImage == null)
            m_thisImage = GetComponent<Image>();

        if (m_defaultInventorySlotSprite == null)
            m_defaultInventorySlotSprite = m_thisImage.sprite;
    }

    // Set the item contained in this item slot.
    public void SetItem(Item item)
    {
        if (item == null)
            return;

        m_slotCurrentItem = item;

        if (m_thisImage != null)
            m_thisImage.sprite = item.m_itemInventorySprite;
    }

    // Remove the item from this item slot.
    public void RemoveItem()
    {
        m_slotCurrentItem = null;

        if (m_defaultInventorySlotSprite != null)
            m_thisImage.sprite = m_defaultInventorySlotSprite;
        else
            m_thisImage.sprite = null;
    }
}
