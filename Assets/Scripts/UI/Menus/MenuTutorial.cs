using UnityEngine;

public class MenuTutorial : Menu
{
    // The timescale that was active when this scene was entered.
    private float m_previousTimeScale = 0.0f;

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

    // Callback triggered by the MenuManager stack.
    public void OnReturnSelected()
    {
        if (MenuManager.instance != null)
            MenuManager.instance.PopMenu();
    }
}
