using UnityEngine;

public class MenuGameOver : Menu
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

    // Callback triggered by the Return To Main Button.
    public void OnReturnMainSelected()
    {
        if (GameManager.instance != null)
            StartCoroutine(GameManager.instance.RestartGame());
    }

    // Callback triggered by the Quit Button.
    public void OnQuitSelected()
    {
        if (GameManager.instance != null)
            GameManager.instance.QuitGame();
    }
}
