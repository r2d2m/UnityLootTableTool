using UnityEngine;

public class MenuMain : Menu
{
    [Tooltip("The menu to push when the Tutorial button is selected.")]
    [SerializeField] private Menu m_tutorialMenu = null;

    // Callback triggered by the MenuManager stack.
    public override void OnEnterMenu()
    {
        // Pause the game.
        Time.timeScale = 0;
    }

    // Callback triggered by the MenuManager stack.
    public override void OnExitMenu()
    {
        // Play the game. This could trigger when entering the tutorial menu,
        // but the tutorial sets this back to zero, so it is fine. There is
        // probably a better way to handle this...
        Time.timeScale = 1.0f;
    }

    // Callback triggered by the Play Button.
    public void OnPlaySelected()
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

    // Callback triggered by the Quit Button.
    public void OnQuitSelected()
    {
        if (GameManager.instance != null)
            GameManager.instance.QuitGame();
    }
}
