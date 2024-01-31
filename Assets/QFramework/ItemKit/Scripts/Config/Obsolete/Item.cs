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
        [DisplayLabel("����")]    // QFramework ���ṩ�Ĺ���
        public string Name = string.Empty;
        [DisplayLabel("�ؼ���")]
        public string Key = string.Empty;
        [DisplayLabel("����")]
        public string Description = string.Empty;
        public Sprite Icon = null;

        [DisplayLabel("�ɶѵ�")]
        public bool IsStackable = true;
        [DisplayIf(nameof(IsStackable), false, true)]
        [DisplayLabel("  �����ѵ���")]
        public bool HasMaxStackableCount = false;
        [DisplayIf(new string[] { nameof(IsStackable), nameof(HasMaxStackableCount) }, new[] { false, false }, true)]
        [DisplayLabel("    ���ѵ���")]
        public int MaxStackableCount = 99;

        [DisplayLabel("������")]
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
            // ���� itemConfig ���õĶ�������Ա༭������
            serializedObject.DrawProperties(false, 0, "Icon", "Name", "Key", "IsStackable", "HasMaxStackableCount", "MaxStackableCount", "IsWeapon");

            //EditorGUILayout.PrefixLabel("ͼ��");
            mIcon.objectReferenceValue = EditorGUILayout.ObjectField(mIcon.objectReferenceValue, typeof(Sprite), true, GUILayout.Height(48), GUILayout.Width(48));
            GUILayout.EndVertical();

            //GUILayout.Space();

            // ����ԭʼ��labelWidth
            float originalLabelWidth = EditorGUIUtility.labelWidth;
            // ����ԭʼ��fieldWidth
            float originalFieldWidth = EditorGUIUtility.fieldWidth;

            // �����µ�labelWidth��fieldWidth
            EditorGUIUtility.labelWidth = 100;
            EditorGUIUtility.fieldWidth = 200;

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            mName = serializedObject.FindProperty("Name");
            // �����������Եı༭��
            EditorGUILayout.PropertyField(mName, GUILayout.ExpandWidth(true)); // �����ֶ��Զ���չ��������Ŀռ�
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            mKey = serializedObject.FindProperty("Key");
            // ���ƹؼ�ֵ���Եı༭��
            EditorGUILayout.PropertyField(mKey, GUILayout.ExpandWidth(true)); // ͬ��
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            mIsStackable = serializedObject.FindProperty("IsStackable");
            // ���ƹؼ�ֵ���Եı༭��
            EditorGUILayout.PropertyField(mIsStackable, GUILayout.ExpandWidth(true)); // ͬ��
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            mHasMaxStackableCount = serializedObject.FindProperty("HasMaxStackableCount");
            // ���ƹؼ�ֵ���Եı༭��
            EditorGUILayout.PropertyField(mHasMaxStackableCount, GUILayout.ExpandWidth(true)); // ͬ��
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            mMaxStackableCount = serializedObject.FindProperty("MaxStackableCount");
            // ���ƹؼ�ֵ���Եı༭��
            EditorGUILayout.PropertyField(mMaxStackableCount, GUILayout.ExpandWidth(true)); // ͬ��
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            mWeapon = serializedObject.FindProperty("IsWeapon");
            // ���ƹؼ�ֵ���Եı༭��
            EditorGUILayout.PropertyField(mWeapon, GUILayout.ExpandWidth(true)); // ͬ��
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            // �ָ�ԭʼ��labelWidth��fieldWidth
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
