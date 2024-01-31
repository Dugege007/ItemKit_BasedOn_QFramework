using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEditor;

#if UNITY_EDITOR
using UnityEngine;
#endif

namespace QFramework.Obsolete
{
    public class Item : ScriptableObject // ,IItem
    {
        [DisplayLabel("名称")]    // QFramework 中提供的功能
        public string Name = string.Empty;
        [DisplayLabel("关键字")]
        public string Key = string.Empty;
        [DisplayLabel("描述")]
        public string Description = string.Empty;
        public Sprite Icon = null;

        [DisplayLabel("可堆叠")]
        public bool IsStackable = true;
        [DisplayIf(nameof(IsStackable), false, true)]
        [DisplayLabel("  有最大堆叠数")]
        public bool HasMaxStackableCount = false;
        [DisplayIf(new string[] { nameof(IsStackable), nameof(HasMaxStackableCount) }, new[] { false, false }, true)]
        [DisplayLabel("    最大堆叠数")]
        public int MaxStackableCount = 99;

        [DisplayLabel("是武器")]
        public bool IsWeapon = false;

        public string GetName => ItemKit.CurrentLanguage == ItemKit.DefaultLanguage ? Name : LocalItem.Name;
        public string GetKey => Key;
        public string GetDescription => ItemKit.CurrentLanguage == ItemKit.DefaultLanguage ? Description : LocalItem.Description;
        public Sprite GetIcon => Icon;
        public bool GetStackable => IsStackable;
        public bool GetHasMaxStackableCount => HasMaxStackableCount;
        public int GetMaxStackableCount => MaxStackableCount;
        public ItemLanguagePackage.LocalItem LocalItem { get; set; }

        public bool GetBoolean(string propertyName)
        {
            if (propertyName == "IsWeapon")
            {
                return IsWeapon;
            }
            return false;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Item))]
    public class ItemConfigEditor : Editor
    {
        private SerializedProperty mIcon;
        private SerializedProperty mName;
        private SerializedProperty mKey;
        private SerializedProperty mIsStackable;
        private SerializedProperty mHasMaxStackableCount;
        private SerializedProperty mMaxStackableCount;
        private SerializedProperty mWeapon;

        private void OnEnable()
        {
            mIcon = serializedObject.FindProperty("Icon");
            mName = serializedObject.FindProperty("Name");
            mKey = serializedObject.FindProperty("Key");
            mIsStackable = serializedObject.FindProperty("IsStackable");
            mHasMaxStackableCount = serializedObject.FindProperty("HasMaxStackableCount");
            mMaxStackableCount = serializedObject.FindProperty("MaxStackableCount");
            mWeapon = serializedObject.FindProperty("IsWeapon");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(80));
            // 绘制 itemConfig 引用的对象的属性编辑器界面
            serializedObject.DrawProperties(false, 0, "Icon", "Name", "Key", "IsStackable", "HasMaxStackableCount", "MaxStackableCount", "IsWeapon");

            //EditorGUILayout.PrefixLabel("图标");
            mIcon.objectReferenceValue = EditorGUILayout.ObjectField(mIcon.objectReferenceValue, typeof(Sprite), true, GUILayout.Height(48), GUILayout.Width(48));
            GUILayout.EndVertical();

            //GUILayout.Space();

            // 保存原始的labelWidth
            float originalLabelWidth = EditorGUIUtility.labelWidth;
            // 保存原始的fieldWidth
            float originalFieldWidth = EditorGUIUtility.fieldWidth;

            // 设置新的labelWidth和fieldWidth
            EditorGUIUtility.labelWidth = 100;
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

            GUILayout.BeginHorizontal();
            mIsStackable = serializedObject.FindProperty("IsStackable");
            // 绘制关键值属性的编辑器
            EditorGUILayout.PropertyField(mIsStackable, GUILayout.ExpandWidth(true)); // 同上
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            mHasMaxStackableCount = serializedObject.FindProperty("HasMaxStackableCount");
            // 绘制关键值属性的编辑器
            EditorGUILayout.PropertyField(mHasMaxStackableCount, GUILayout.ExpandWidth(true)); // 同上
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            mMaxStackableCount = serializedObject.FindProperty("MaxStackableCount");
            // 绘制关键值属性的编辑器
            EditorGUILayout.PropertyField(mMaxStackableCount, GUILayout.ExpandWidth(true)); // 同上
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            mWeapon = serializedObject.FindProperty("IsWeapon");
            // 绘制关键值属性的编辑器
            EditorGUILayout.PropertyField(mWeapon, GUILayout.ExpandWidth(true)); // 同上
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            // 恢复原始的labelWidth和fieldWidth
            EditorGUIUtility.labelWidth = originalLabelWidth;
            EditorGUIUtility.fieldWidth = originalFieldWidth;

            GUILayout.EndHorizontal();

            if (target.name != mKey.stringValue)
            {
                target.name = mKey.stringValue;
                EditorUtility.SetDirty(target);
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
#endif
}
