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
        [DisplayLabel("ͼ��")]
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
            // ���� itemConfig ���õĶ�������Ա༭������
            serializedObject.DrawProperties(false, 0, "Icon", "Name", "Key");

            EditorGUILayout.PrefixLabel("ͼ��");
            mIcon.objectReferenceValue = EditorGUILayout.ObjectField(mIcon.objectReferenceValue, typeof(Sprite), true, GUILayout.Height(48), GUILayout.Width(48));
            GUILayout.EndVertical();

            GUILayout.Space(-60);

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
