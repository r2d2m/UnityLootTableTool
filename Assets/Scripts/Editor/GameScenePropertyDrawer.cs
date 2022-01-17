using UnityEngine;
using UnityEditor;

// PROVIDED BY DEMARK IN GAP 351

[CustomPropertyDrawer(typeof(GameScene))]
public class GameScenePropertyDrawer : PropertyDrawer
{
    private static readonly string s_kSceneProperty = "m_scene";
    private static readonly string s_kNameProperty = "m_name";

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label = EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, label);

        EditorGUI.BeginChangeCheck();
        SerializedProperty sceneProperty = property.FindPropertyRelative(s_kSceneProperty);
        SerializedProperty nameProperty = property.FindPropertyRelative(s_kNameProperty);

        int indentLevel = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        EditorGUI.ObjectField(position, sceneProperty, typeof(SceneAsset), GUIContent.none);

        if (EditorGUI.EndChangeCheck())
        {
            nameProperty.stringValue = (sceneProperty.objectReferenceValue as SceneAsset).name;
            property.serializedObject.ApplyModifiedProperties();
        }

        EditorGUI.indentLevel = indentLevel;

        EditorGUI.EndProperty();
    }
}

