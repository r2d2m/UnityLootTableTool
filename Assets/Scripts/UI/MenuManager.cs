using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    private static readonly string s_kDoubleAwakeErrorText = "Awake() has been called on MenuManager when the MenuManager already Exists.";

    // Stack containing all active menus.
    private Stack<Menu> m_menuStack = new Stack<Menu>();

    [Tooltip("The initial menu to push when the game begins.")]
    [SerializeField] private Menu m_initialMenu = null;

    // The public accessor of this MenuManager Singleton.
    public static MenuManager instance { get; private set; }

    private void Awake()
    {
        // Spin up the singleton instance.
        if (instance != null)
        {
            GameObject.Destroy(this.gameObject);
            Debug.LogError(s_kDoubleAwakeErrorText);
            return;
        }
        instance = this;

        if (m_initialMenu == null)
        {
            // Create Main Menu UI Instance
            m_initialMenu = null;
        }

        Debug.Assert(m_initialMenu != null);
        PushMenu(m_initialMenu);
    }

    // Add a menu to the top of the stack. This menu will now be visible over the others in the stack.
    public void PushMenu(Menu menu)
    {
        Menu currentMenu = null;

        if (m_menuStack.Count > 0)
            currentMenu = m_menuStack.Peek();

        if (currentMenu != null)
        {
            currentMenu.OnExitMenu();
            currentMenu.gameObject.SetActive(false);
        }

        m_menuStack.Push(menu);

        menu.gameObject.SetActive(true);
        menu.OnEnterMenu();
    }

    // Pop the top menu off the stack.
    public void PopMenu()
    {
        Menu currentMenu = m_menuStack.Pop();

        if (currentMenu != null)
        {
            currentMenu.OnExitMenu();
            currentMenu.gameObject.SetActive(false);
            currentMenu = null;
        }

        if (m_menuStack.Count > 0)
            currentMenu = m_menuStack.Peek();

        if (currentMenu != null)
        {
            currentMenu.OnEnterMenu();
            currentMenu.gameObject.SetActive(true);
        }
    }

    // Check if the stack is empty.
    public bool IsEmpty()
    {
        return m_menuStack.Count == 0;
    }
}
