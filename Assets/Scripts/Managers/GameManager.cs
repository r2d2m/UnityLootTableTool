using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static readonly string s_kDoubleAwakeErrorText = "Awake() has been called on GameManager when the GameManager already Exists.";
    private static readonly string s_kLoadProgress = "Progress: {0}";

    [Tooltip("The scene that will be loaded on Init.")]
    [SerializeField] private GameScene m_initialLoadScene = null;

    [Tooltip("The load screen that the game uses.")]
    [SerializeField] private LoadScreen m_loadScreen = null;

    // The public accessor of this GameManager Singleton.
    public static GameManager instance { get; private set; }

    private IEnumerator Start()
    {
        Debug.Assert(m_initialLoadScene != null);

        // Spin up the singleton instance.
        if (instance != null)
        {
            GameObject.Destroy(this.gameObject);
            Debug.LogError(s_kDoubleAwakeErrorText);
            yield break;
        }

        instance = this;
        GameObject.DontDestroyOnLoad(this.gameObject);

        yield return RestartGame();
    }

    // Do I need to explicitly call this when I shut the game down, or will unity do it?
    public void DestroyGameManager()
    {
        GameObject.Destroy(this.gameObject);
        instance = null;
    }

    // Close the application or stop running the editor.
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Not really sure how I can prevent copy-pasting this code.
    // I'm sure its trivial.
    public IEnumerator RestartGame()
    {
        // Load the starting scene, not using the load screen.
        if (m_loadScreen == null)
        {
            SceneManager.LoadScene(m_initialLoadScene.m_name);
            yield break;
        }
        
        // Use laod screen.
        WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();

        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(m_initialLoadScene.m_name);
        while (!asyncOp.isDone)
        {
            Debug.Log(string.Format(s_kLoadProgress, asyncOp.progress));
            m_loadScreen.SetFillPercent(asyncOp.progress);
            yield return endOfFrame;
        }

        m_loadScreen.gameObject.SetActive(false);
        asyncOp.allowSceneActivation = true;
    }
}
