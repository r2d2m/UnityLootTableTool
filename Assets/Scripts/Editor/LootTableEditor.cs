using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class LootTableEditor : EditorWindow
{
    // The cached window reference.
    private static LootTableEditor m_lootTableEditor = null;

    // Specific window size.
    private const float kWindowWidth = 640.0f;
    private const float kWindowHeight = 480.0f;

    // Cached strings
    private static readonly string s_kPathAppender          = "/Resources/Items/LootTables";
    private static readonly string s_kFileExtention         = "txt";
    private static readonly string s_kDefaultTableName      = "New Loot Table";
    private static readonly string s_kLoadedTableLabelField = "Loot Table:";

    // Loading
    private static readonly string s_kLoadButtonText        = "Load Loot Table";
    private static readonly string s_kLoadFilePanelTitle    = "Open Loot Table to Load";

    // Saving
    private static readonly string s_kSaveFilePanelTitle    = "Save Loot Table as";
    private static readonly string s_kSaveButtonText        = "Save Loot Table";
    private static readonly string s_kSaveExitButtonText    = "Save Loot Table and Exit";
    private static readonly string s_kCancelExitButtonText  = "Cancel and Exit";
    private static readonly string s_kClearButtonText       = "Clear Table";

    // Log Messages
    private static readonly string s_kEditorInvalidText     = "Loot Table Editor reference was Invalid.";
    private static readonly string s_kOpenFailedText        = "Opening Loot Table '{0}' failed.";
    private static readonly string s_kOpenEmptyText         = "Opened Loot Table '{0}' was empty.";
    private static readonly string s_kBadTableText          = "Loot Table '{0}' was not a LootTable or was corrupt.";
    private static readonly string s_kInvalidSavePathText   = "Save path '{0}' is not a valid path. You can safely ignore this if you pressed cancel.";
    private static readonly string s_kOverwriteTitleText    = "Overwrite Loot Table?";
    private static readonly string s_kOverwriteBodyText     = "Are you sure you want to overwrite this Loot Table? This cannot be undone.";
    private static readonly string s_kYesText               = "Yes";
    private static readonly string s_kNoText                = "No";
    private static readonly string s_kSerializeFailedText   = "Loot Table '{0}' failed to serialize.";
    private static readonly string s_kNullTableText         = "LootTable was Null. Something unexpected has occured.";
    private static readonly string s_kUnmatchedTablesText   = "LootTable count does not match ReorderedList count.";
    private static readonly string s_kSuccessText           = "Loot Table Saved Successfully.";

    // ReorderableList Labels
    private static readonly string s_kReorderableListHeader = "Items";
    private static readonly string s_kItemPrefabLabel       = "Item Prefab:";
    private static readonly string s_kSpriteLabel           = "Inventory Icon:";
    private static readonly string s_kWeightLabel           = "Item Weight:";

    // ReorderableList GUI Sizing Tunables
    private const float kItemPrefabLabelOffset              = 0.0f;
    private const float kItemPrefabLabelWidth               = 70.0f;
    private const float kItemPrefabFieldOffset              = 70.0f;
    private const float kItemPrefabFieldWidth               = 100.0f;

    private const float kSpriteLabelOffset                  = 175.0f;
    private const float kSpriteLabelWidth                   = 90.0f;
    private const float kSpriteFieldOffset                  = 265.0f;
    private const float kSpriteFieldWidth                   = 100.0f;

    private const float kWeightLabelOffset                  = 370.0f;
    private const float kWeightLabelWidth                   = 70.0f;
    private const float kWeightFieldOffset                  = 445.0f;
    private const float kWeightFieldWidth                   = 170.0f;

    // Runtime populated strings
    private string m_applicationDataPath                    = "";
    private string m_currentLootTablePath                   = s_kDefaultTableName;

    // Runtime populated 'structures'. These don't *really* need to be separate
    // we can use m_reorderedItemList.list anywhere that m_currentLootItem is
    // used instead, but I think the separation is a little more clear and it
    // makes the m_reorderedItemList initialization cleaner. This is probably
    // a matter of opinion, though.
    private LootTable m_currentLootTable = null;
    private ReorderableList m_reorderedItemList = null;

    [MenuItem("Tools/LootTableEditor &l")]
    public static void OpenWindow()
    {
        m_lootTableEditor = EditorWindow.CreateInstance<LootTableEditor>();

        SetWindowSize();

        m_lootTableEditor.ShowUtility();
    }

    // Force the window to set size.
    private static void SetWindowSize()
    {
        m_lootTableEditor.maxSize = new Vector2(kWindowWidth, kWindowHeight);
        m_lootTableEditor.minSize = m_lootTableEditor.maxSize;
    }

    public void OnGUI()
    {
        InitializeLootEditor();

        DrawLoadTableGUI();

        DrawReorderedItemList();

        DrawSaveTableGUI();
    }

    // Create any member variables if needed.
    private void InitializeLootEditor()
    {
        if (m_lootTableEditor == null)
            Debug.LogError(s_kEditorInvalidText);

        if (m_currentLootTable == null)
            m_currentLootTable = new LootTable();

        if (m_reorderedItemList == null)
        {
            m_reorderedItemList = new ReorderableList(m_currentLootTable.m_items, typeof(Item), true, true, true, true);
            m_reorderedItemList.drawHeaderCallback = DrawItemListHeader;
            m_reorderedItemList.drawElementCallback = DrawItemListElements;
            m_reorderedItemList.onAddCallback = AddItemToList;
        }

        if (string.IsNullOrEmpty(m_applicationDataPath))
            m_applicationDataPath = Application.dataPath;
    }    

    // Draw the GUI for the Loading button.
    private void DrawLoadTableGUI()
    {
        GUILayout.BeginHorizontal();
        {
            EditorGUILayout.TextField(s_kLoadedTableLabelField, m_currentLootTablePath);
            if (GUILayout.Button(s_kLoadButtonText))
                TryLoadLootTable();
        }
        GUILayout.EndHorizontal();
    }

    // Draw the GUI for the Save, Save and Exit, Clear and Cancel buttons.
    private void DrawSaveTableGUI()
    {
        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button(s_kSaveButtonText))
                TrySaveChanges();
            else if (GUILayout.Button(s_kSaveExitButtonText))
            {
                TrySaveChanges();
                CloseEditorWindow();
            }
            else if (GUILayout.Button(s_kClearButtonText))
                ResetLootTableEditor();
            else if (GUILayout.Button(s_kCancelExitButtonText))
                CloseEditorWindow();
        }
        GUILayout.EndHorizontal();
    }

    // Draw the up-to-date reorderable list.
    private void DrawReorderedItemList()
    {
        if (m_reorderedItemList != null)
            m_reorderedItemList.DoLayoutList();
    }

    // Attempt to load a loot table from an Openfile popup.
    private void TryLoadLootTable()
    {
        if (!string.IsNullOrEmpty(m_applicationDataPath))
            m_currentLootTablePath = EditorUtility.OpenFilePanel(s_kLoadFilePanelTitle, m_applicationDataPath + s_kPathAppender, s_kFileExtention);

        if (string.IsNullOrEmpty(m_currentLootTablePath) || !File.Exists(m_currentLootTablePath))
        {
            Debug.LogError(string.Format(s_kOpenFailedText, m_currentLootTablePath));
            return;
        }

        string jsonData = File.ReadAllText(m_currentLootTablePath);

        if (string.IsNullOrEmpty(jsonData))
        {
            Debug.LogWarning(string.Format(s_kOpenEmptyText, m_currentLootTablePath));
            return;
        }

        if (m_currentLootTable == null)
        {
            Debug.LogError(s_kNullTableText);
            return;
        }

        JsonUtility.FromJsonOverwrite(jsonData, m_currentLootTable);

        if (m_currentLootTable == null || m_currentLootTable.m_items == null)
        {
            Debug.LogWarning(string.Format(s_kBadTableText, m_currentLootTablePath));
            return;
        }
    }

    // Attempt to save or overwrite any changes made to a table using a Savefile popup.
    private void TrySaveChanges()
    {
        if (!string.IsNullOrEmpty(m_currentLootTablePath) && File.Exists(m_currentLootTablePath))
        {
            // Overwrite
            PerformTableSave();
            return;
        }

        if (!string.IsNullOrEmpty(m_applicationDataPath))
            m_currentLootTablePath = EditorUtility.SaveFilePanel(s_kSaveFilePanelTitle, m_applicationDataPath + s_kPathAppender, s_kDefaultTableName, s_kFileExtention);

        if (string.IsNullOrEmpty(m_currentLootTablePath))
        {
            Debug.LogError(string.Format(s_kInvalidSavePathText, m_currentLootTablePath));
            m_currentLootTablePath = s_kDefaultTableName;
            return;
        }

        if (File.Exists(m_currentLootTablePath))
        {
            bool doOverwrite = EditorUtility.DisplayDialog(s_kOverwriteTitleText, s_kOverwriteBodyText, s_kYesText, s_kNoText);
            if (!doOverwrite)
                return; // We cancel this save attempt.
        }

        PerformTableSave();
    }

    // Save the current loot table to a text file.
    private void PerformTableSave()
    {
        if (m_currentLootTable == null)
        {
            Debug.LogError(s_kNullTableText);
            return;
        }
        m_currentLootTable.SerializeItems();

        string jsonData = JsonUtility.ToJson(m_currentLootTable);

        if (string.IsNullOrEmpty(jsonData))
        {
            Debug.LogWarning(string.Format(s_kSerializeFailedText, m_currentLootTablePath));
            return;
        }

        File.WriteAllText(m_currentLootTablePath, jsonData);

        Debug.Log(s_kSuccessText);
    }

    // Clears and closes the editor.
    private void CloseEditorWindow()
    {
        ResetLootTableEditor();

        if (m_lootTableEditor != null)
            m_lootTableEditor.Close();
        else
            Debug.LogError(s_kEditorInvalidText);
    }

    // Clear the editor.
    private void ResetLootTableEditor()
    {
        m_currentLootTablePath = s_kDefaultTableName;
        m_reorderedItemList = null;
        m_currentLootTable = null;
    }

    // Callback for handling drawing the reorderable list header.
    private void DrawItemListHeader(Rect rect)
    {
        EditorGUI.LabelField(rect, s_kReorderableListHeader, EditorStyles.boldLabel);
    }

    // Callback for handling drawing a reorderable list element.
    private void DrawItemListElements(Rect rect, int index, bool isActive, bool isFocused)
    {
        if (m_reorderedItemList.list == null)
            return; // Nothing to draw.

        if (index > m_reorderedItemList.count || index > m_currentLootTable.m_items.Count)
            return; // Prevent out of bounds errors.

        // Get the item in the list at this index. 'null' is valid here as it translates to None in the editor window.
        Item element = (Item)m_reorderedItemList.list[index];

        // Item prefab field.
        EditorGUI.LabelField(new Rect(rect.x + kItemPrefabLabelOffset, rect.y, kItemPrefabLabelWidth, EditorGUIUtility.singleLineHeight), s_kItemPrefabLabel);
        element.m_itemGameObject = (GameObject)EditorGUI.ObjectField(new Rect(rect.x + kItemPrefabFieldOffset, rect.y, kItemPrefabFieldWidth, EditorGUIUtility.singleLineHeight), element.m_itemGameObject, typeof(GameObject), false);
        
        // Sprite field.
        EditorGUI.LabelField(new Rect(rect.x + kSpriteLabelOffset, rect.y, kSpriteLabelWidth, EditorGUIUtility.singleLineHeight), s_kSpriteLabel);
        element.m_itemInventorySprite = (Sprite)EditorGUI.ObjectField(new Rect(rect.x + kSpriteFieldOffset, rect.y, kSpriteFieldWidth, EditorGUIUtility.singleLineHeight), element.m_itemInventorySprite, typeof(Sprite), false);

        // Weight field
        EditorGUI.LabelField(new Rect(rect.x + kWeightLabelOffset, rect.y, kWeightLabelWidth, EditorGUIUtility.singleLineHeight), s_kWeightLabel);
        element.m_itemDropRate = EditorGUI.Slider(new Rect(rect.x + kWeightFieldOffset, rect.y, kWeightFieldWidth, EditorGUIUtility.singleLineHeight), element.m_itemDropRate, 0.0f, 1.0f);

        if (m_currentLootTable == null)
        {
            Debug.LogError(s_kNullTableText);
            return;
        }

        m_currentLootTable.m_items[index] = element;
    }

    // Callback for handling adding elements to the reordeable list throught the '+' button.
    private void AddItemToList(ReorderableList addList)
    {
        if (addList == null || addList.list == null)
            return;

        Item newItem = new Item();

        addList.list.Add(newItem);

        if (m_currentLootTable.m_items.Count != m_reorderedItemList.count)
        {
            Debug.LogError(s_kUnmatchedTablesText);
            return;
        }
    }
}
