#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

[System.Serializable]
public class Item 
{
    public GameObject m_itemGameObject = null;

    public Sprite m_itemInventorySprite = null;

    [Range(0.0f, 1.0f)] public float m_itemDropRate = 0.0f;

    [SerializeField] private string m_itemGameObjectPath = "";

    [SerializeField] private string m_itemSpritePath = "";

    private static readonly string s_kItemPathInvalid   = "Asset Path for Item GameObject is invalid.";
    private static readonly string s_kSpritePathInvalid = "Asset Path for Sprite is invalid.";
    private static readonly string s_kItemLoadFailed    = "Item GameObject failed to load.";
    private static readonly string s_kSpriteLoadFailed  = "Item Sprite failed to load.";
    private static readonly string s_kAssetPathCullStr  = "Assets/Resources/";

#if UNITY_EDITOR
    // Called by the LootTableEditor when a table is saved.
    public void SetSavedProperties()
    {
        // There is likely a better way to do this,
        // but there is equally likely a batter way to
        // rearchitect this entire system.

        // Get the asset path for this asset
        m_itemGameObjectPath = AssetDatabase.GetAssetPath(m_itemGameObject);

        // Strip out directories that are not necessarry for Resources.Load
        if (m_itemGameObjectPath.Contains(s_kAssetPathCullStr))
            m_itemGameObjectPath = m_itemGameObjectPath.Replace(s_kAssetPathCullStr, null);

        // Strip out the file extention, as it will cause Resource.Load to fail.
        m_itemGameObjectPath = System.IO.Path.ChangeExtension(m_itemGameObjectPath, null);

        // Repeat with sprite.
        m_itemSpritePath = AssetDatabase.GetAssetPath(m_itemInventorySprite);
        if (m_itemSpritePath.Contains(s_kAssetPathCullStr))
            m_itemSpritePath = m_itemSpritePath.Replace(s_kAssetPathCullStr, null);
        m_itemSpritePath = System.IO.Path.ChangeExtension(m_itemSpritePath, null);
    }
#endif

    // Load the item based on the asset paths that
    // should have been serialized from the editor.
    public bool InitializeItem()
    {

        if (string.IsNullOrEmpty(m_itemGameObjectPath))
        {
            Debug.LogError(s_kItemPathInvalid);
            return false;
        }

        m_itemGameObject = Resources.Load<GameObject>(m_itemGameObjectPath);

        if (m_itemGameObject == null)
        {
            Debug.LogError(s_kItemLoadFailed);
            return false;
        }

        if (string.IsNullOrEmpty(m_itemSpritePath))
        {
            Debug.LogError(s_kSpritePathInvalid);
            return false;
        }

        m_itemInventorySprite = Resources.Load<Sprite>(m_itemSpritePath);

        if (m_itemInventorySprite == null)
        {
            Debug.LogError(s_kSpriteLoadFailed);
            return false;
        }

        return true;
    }
}
