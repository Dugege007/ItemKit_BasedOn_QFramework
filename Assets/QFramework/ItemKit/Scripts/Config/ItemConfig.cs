using UnityEditor;

#if UNITY_EDITOR
using UnityEngine;
#endif

namespace QFramework
{
    [CreateAssetMenu(menuName = "@ItemKit/Create Item")]
    public class ItemConfig : ScriptableObject, IItem
    {
        [DisplayLabel("����")]    // QFramework ���ṩ�Ĺ���
        public string Name = string.Empty;
        [DisplayLabel("�ؼ���")]
        public string Key = string.Empty;

        public Sprite Icon = null;

        [DisplayLabel("������")]
        public bool IsWeapon = false;

        public string GetName => Name;
        public string GetKey => Key;
        public Sprite GetIcon => Icon;

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
    [CustomEditor(typeof(ItemConfig))]
    public class ItemConfigEditor : Editor
    {
        private SerializedProperty mIcon;
        private SerializedProperty mName;
        private SerializedProperty mKey;
        private SerializedProperty mWeapon;

        private void OnEnable()
        {
            mIcon = serializedObject.FindProperty("Icon");
            mName = serializedObject.FindProperty("Name");
            mKey = serializedObject.FindProperty("Key");
            mWeapon = serializedObject.FindProperty("IsWeapon");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(80));
            // ���� itemConfig ���õĶ�������Ա༭������
            serializedObject.DrawProperties(false, 0, "Icon", "Name", "Key", "IsWeapon");

            //EditorGUILayout.PrefixLabel("ͼ��");
            mIcon.objectReferenceValue = EditorGUILayout.ObjectField(mIcon.objectReferenceValue, typeof(Sprite), true, GUILayout.Height(48), GUILayout.Width(48));
            GUILayout.EndVertical();

            //GUILayout.Space();

            // ����ԭʼ��labelWidth
            float originalLabelWidth = EditorGUIUtility.labelWidth;
            // ����ԭʼ��fieldWidth
            float originalFieldWidth = EditorGUIUtility.fieldWidth;

            // �����µ�labelWidth��fieldWidth
            EditorGUIUtility.labelWidth = 60;
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
            mWeapon = serializedObject.FindProperty("IsWeapon");
            // ���ƹؼ�ֵ���Եı༭��
            EditorGUILayout.PropertyField(mWeapon, GUILayout.ExpandWidth(true)); // ͬ��
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            // �ָ�ԭʼ��labelWidth��fieldWidth
            EditorGUIUtility.labelWidth = originalLabelWidth;
            EditorGUIUtility.fieldWidth = originalFieldWidth;

            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
#endif
}
