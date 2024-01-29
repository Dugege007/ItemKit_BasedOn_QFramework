using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    [CreateAssetMenu(menuName = "@ItemKit/Create ItemConfig")]
    public class ItemConfig : ScriptableObject, IItem
    {
        [HideLabel]
        [PreviewField(48, ObjectFieldAlignment.Left)]
        [HorizontalGroup("��������", 54), VerticalGroup("��������/left")]
        public Sprite Icon = null;

        [Button]
        private void X()
        {
            Destroy(this);
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
        [VerticalGroup("����/stackable")]
        [LabelText("�ɶѵ�")]
        public bool IsStackable = true;
        [ShowIf("IsStackable")]
        [VerticalGroup("����/stackable")]
        [LabelText("  �����ֵ")]
        public bool HasMaxStackableCount = false;
        [ShowIf("IsStackable"), EnableIf("HasMaxStackableCount")]
        [DisplayIf(new string[] { "IsStackable", "HasMaxStackableCount" }, new[] { false, false })]
        [VerticalGroup("����/stackable")]
        [LabelText("    ���ֵ")]
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
