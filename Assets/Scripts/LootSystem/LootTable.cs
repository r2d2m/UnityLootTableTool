using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LootTable
{
    // LootTables fail to serialize if a property is used.
    // Not sure I understand why.
    //public List<Item> items { get { return m_items; } }
    //private List<Item> m_items = new List<Item>();

    public List<Item> m_items = new List<Item>();

    // Get a random drop based on the weights of the items in the list.
    // This is not currently very ideal, but I ran out of time before I
    // could make a smarter system. Ideally, I would want the editor to
    // force the total weights of all the items to sum to 1.0f. At the
    // very least, I would change this to calculate the total weights
    // once during an 'Awake' function after the table is constructed.
    public Item GetWeightedRandomDrop()
    {
        if (m_items.Count <= 0)
            return null;

        // :( see comment above
        float totalWeight = 0.0f;
        for (int i = 0; i < m_items.Count; ++i)
        {
            totalWeight += m_items[i].m_itemDropRate;
        }

        float chance = Random.Range(0.0f, totalWeight);

        for (int i = 0; i < m_items.Count; ++i)
        {
            chance -= m_items[i].m_itemDropRate;

            if (chance <= 0.0f)
                return m_items[i];
        }

        return m_items[m_items.Count - 1];
    }

    public bool InitializeLootTable()
    {
        bool result = true;
        for (int i = 0; i < m_items.Count; ++i)
        {
            result = m_items[i].InitializeItem();
        }
        return result;
    }

#if UNITY_EDITOR
    public void SerializeItems()
    {
        for (int i = 0; i < m_items.Count; ++i)
        {
            m_items[i].SetSavedProperties();
        }
    }
#endif
}
