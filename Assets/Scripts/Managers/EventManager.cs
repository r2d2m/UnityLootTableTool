using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EventID
{
    // UI Events
    public static readonly string s_kPauseGame = "PauseGame";
    public static readonly string s_kToggleInventoryUIEvent = "InventoryToggled";
    public static readonly string s_kInventorySlotCleared = "InventorySlotCleared";

    // Animation Events
    public static readonly string s_kCharacterMovedEvent = "CharacterMoved";
    public static readonly string s_kCharacterStoppedEvent = "CharacterStopped";
    public static readonly string s_kCharacterAttackedEvent = "CharacterAttacked";

    // Player Inventory Events
    public static readonly string s_kItemCollectionEvent = "ItemCollection"; // Item interacted with.
}

[System.Serializable]
public class EventData
{
    public GameObject m_triggerObject { get; set; }
    public GameObject m_targetObject { get; set; }
    public Vector3 m_targetPosition { get; set; }
}

public delegate void EventCallback(EventData data = null);

public static class EventManager
{
    private static Dictionary<string, EventCallback> m_eventListeners = new Dictionary<string, EventCallback>();

    public static void TriggerEvent(string eventID, EventData data = null)
    {
        if (m_eventListeners.ContainsKey(eventID))
            m_eventListeners[eventID].Invoke(data);
    }

    public static void AddListener(string eventID, EventCallback callback)
    {
        if (!m_eventListeners.ContainsKey(eventID))
            m_eventListeners.Add(eventID, callback);
        else
            m_eventListeners[eventID] += callback;
    }

    public static void RemoveListener(string eventID, EventCallback callback)
    {
        if (m_eventListeners.ContainsKey(eventID))
        {
            m_eventListeners[eventID] -= callback;
            if (m_eventListeners[eventID] == null)
                m_eventListeners.Remove(eventID);
        }
    }
}
