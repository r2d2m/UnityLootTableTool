using UnityEngine;

public class MenuPause : Menu
{
    // The timescale that was active when this scene was entered.
    private float m_previousTimeScale = 0.0f;

    [Tooltip("The menu to push when the Tutorial button is selected.")]
    [SerializeField] private Menu m_tutorialMenu = null;

    [Tooltip("The menu to push when the Quit button is selected.")]
    [SerializeField] private Menu m_gameOverMenu = null;

    // Cached this. May not be super necessary, as it's not regularly called.
    private GameObject m_thisObject = null;

    private void Awake()
    {
        EventManager.AddListener(EventID.s_kPauseGame, OnPauseGame);

        m_thisObject = this.gameObject;
        m_thisObject.SetActive(false);
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener(EventID.s_kPauseGame, OnPauseGame);
    }

    // Callback triggered by the MenuManager stack.
    public override void OnEnterMenu()
    {
        m_previousTimeScale = Time.timeScale;
        Time.timeScale = 0;
    }

    // Callback triggered by the MenuManager stack.
    public override void OnExitMenu()
    {
        Time.timeScale = m_previousTimeScale;
    }

    // Callback triggered by the Return Button.
    public void OnReturnSelected()
    {
        if (MenuManager.instance != null)
            MenuManager.instance.PopMenu();
    }

    // Callback triggered by the Tutorial Button.
    public void OnTutorialSelected()
    {
        if (m_tutorialMenu == null)
            return;

        if (MenuManager.instance != null)
            MenuManager.instance.PushMenu(m_tutorialMenu);
    }

    // Callback triggered by the Return To Main Button.
    public void OnReturnMainSelected()
    {
        if (GameManager.instance != null)
             StartCoroutine(GameManager.instance.RestartGame());
    }

    // Callback triggered by the Quit Button.
    public void OnQuitSelected()
    {
        if (m_gameOverMenu == null)
            return;

        if (MenuManager.instance != null)
            MenuManager.instance.PushMenu(m_gameOverMenu);
    }

    // Callback triggered by the Pause Event, Toggles the pause menu.
    private void OnPauseGame(EventData data)
    {
        if (MenuManager.instance && MenuManager.instance.IsEmpty())
            MenuManager.instance.PushMenu(this);
        else if (m_thisObject.activeInHierarchy)
            MenuManager.instance.PopMenu();
    }
}
