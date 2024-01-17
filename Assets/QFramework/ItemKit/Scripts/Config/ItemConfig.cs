using UnityEditor;

#if UNITY_EDITOR
using UnityEngine;
#endif

namespace QFramework
{
    [CreateAssetMenu(menuName = "@ItemKit/Create Item")]
    public class ItemConfig : ScriptableObject, IItem
    {
        [DisplayLabel("名称")]    // QFramework 中提供的功能
        public string Name = string.Empty;
        [DisplayLabel("关键字")]
        public string Key = string.Empty;
        [DisplayLabel("图标")]
        public Sprite Icon = null;

        public string GetName => Name;
        public string GetKey => Key;
        public Sprite GetIcon => Icon;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ItemConfig))]
    public class ItemConfigEditor : Editor
    {
        private SerializedProperty mIcon;
        private SerializedProperty mName;
        private SerializedProperty mKey;

        private void OnEnable()
        {
            mIcon = serializedObject.FindProperty("Icon");
            mName = serializedObject.FindProperty("Name");
            mKey = serializedObject.FindProperty("Key");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(80));
            // 绘制 itemConfig 引用的对象的属性编辑器界面
            serializedObject.DrawProperties(false, 0, "Icon", "Name", "Key");

            EditorGUILayout.PrefixLabel("图标");
            mIcon.objectReferenceValue = EditorGUILayout.ObjectField(mIcon.objectReferenceValue, typeof(Sprite), true, GUILayout.Height(48), GUILayout.Width(48));
            GUILayout.EndVertical();

            GUILayout.Space(-60);

            // 保存原始的labelWidth
            float originalLabelWidth = EditorGUIUtility.labelWidth;
            // 保存原始的fieldWidth
            float originalFieldWidth = EditorGUIUtility.fieldWidth;

            // 设置新的labelWidth和fieldWidth
            EditorGUIUtility.labelWidth = 60;
            EditorGUIUtility.fieldWidth = 200;

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            mName = serializedObject.FindProperty("Name");
            // 绘制名称属性的编辑器
            EditorGUILayout.PropertyField(mName, GUILayout.ExpandWidth(true)); // 允许字段自动扩展以填充额外的空间
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            mKey = serializedObject.FindProperty("Key");
            // 绘制关键值属性的编辑器
            EditorGUILayout.PropertyField(mKey, GUILayout.ExpandWidth(true)); // 同上
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            // 恢复原始的labelWidth和fieldWidth
            EditorGUIUtility.labelWidth = originalLabelWidth;
            EditorGUIUtility.fieldWidth = originalFieldWidth;

            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
#endif
}
