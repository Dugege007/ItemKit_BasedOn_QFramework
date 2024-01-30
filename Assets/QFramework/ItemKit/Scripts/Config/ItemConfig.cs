using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    [CreateAssetMenu(menuName = "@ItemKit/Create ItemConfig")]
    public class ItemConfig : ScriptableObject, IItem
    {
        public ItemConfigGroup ItemConfigGroup { get; set; }

        [HideLabel]
        [PreviewField(48, ObjectFieldAlignment.Left)]
        [HorizontalGroup("��������", 54), VerticalGroup("��������/left")]
        public Sprite Icon = null;

        [VerticalGroup("��������/left")]
        [Button]
        public void Refresh()
        {
            this.name = Key;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [VerticalGroup("��������/left")]
        [Button, GUIColor(1, 0, 0)]
        private void X()
        {
            if (EditorUtility.DisplayDialog("ɾ����Ʒ", "ȷ��Ҫɾ����\n���˲������ɻָ���", "ɾ��", "ȡ��"))
            {
                ItemConfigGroup.ItemConfigs.Remove(this);
                AssetDatabase.RemoveObjectFromAsset(this);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        [VerticalGroup("��������/right"), LabelWidth(42)]
        [LabelText("����")]
        public string Name = string.Empty;
        [VerticalGroup("��������/right"), LabelWidth(42)]
        [LabelText("�ؼ���")]
        public string Key = string.Empty;
        [VerticalGroup("��������/right"), LabelWidth(42)]
        [LabelText("������")]
        public bool IsWeapon = false;

        [HorizontalGroup("����")]
        [VerticalGroup("����/stackable"), LabelWidth(66)]
        [LabelText("�ɶѵ�")]
        public bool IsStackable = true;
        [ShowIf("IsStackable")]
        [VerticalGroup("����/stackable"), LabelWidth(66)]
        [Indent]
        [LabelText("�����ֵ")]
        public bool HasMaxStackableCount = false;
        [ShowIf("IsStackable"), EnableIf("HasMaxStackableCount")]
        [DisplayIf(new string[] { "IsStackable", "HasMaxStackableCount" }, new[] { false, false })]
        [VerticalGroup("����/stackable"), LabelWidth(66)]
        [Indent(2)]
        [LabelText("���ֵ")]
        public int MaxStackableCount = 99;

        public string GetName => Name;
        public string GetKey => Key;
        public Sprite GetIcon => Icon;
        public bool GetStackable => IsStackable;
        public bool GetHasMaxStackableCount => HasMaxStackableCount;
        public int GetMaxStackableCount => MaxStackableCount;

        public bool GetBoolean(string propertyName)
        {
            if (propertyName == "IsWeapon")
            {
                return IsWeapon;
            }
            return false;
        }
    }
}
