#if UNITY_EDITOR
using UnityEditor;
#endif

// PROVIDED BY DEMARK IN GAP 351

[System.Serializable]
public class GameScene
{
#if UNITY_EDITOR
    // The Scene asset to use.
    public SceneAsset m_scene = null;
#endif

    // The scene asset name.
    public string m_name = "";
}
