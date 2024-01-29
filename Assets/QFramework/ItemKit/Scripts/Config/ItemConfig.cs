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
        [HorizontalGroup("名称类型", 54), VerticalGroup("名称类型/left")]
        public Sprite Icon = null;

        [Button]
        private void X()
        {
            Destroy(this);
        }

        [VerticalGroup("名称类型/right"), LabelWidth(42)]
        [LabelText("名称")]
        public string Name = string.Empty;
        [VerticalGroup("名称类型/right"), LabelWidth(42)]
        [LabelText("关键字")]
        public string Key = string.Empty;
        [VerticalGroup("名称类型/right"), LabelWidth(42)]
        [LabelText("是武器")]
        public bool IsWeapon = false;

        [HorizontalGroup("属性")]
        [VerticalGroup("属性/stackable")]
        [LabelText("可堆叠")]
        public bool IsStackable = true;
        [ShowIf("IsStackable")]
        [VerticalGroup("属性/stackable")]
        [LabelText("  有最大值")]
        public bool HasMaxStackableCount = false;
        [ShowIf("IsStackable"), EnableIf("HasMaxStackableCount")]
        [DisplayIf(new string[] { "IsStackable", "HasMaxStackableCount" }, new[] { false, false })]
        [VerticalGroup("属性/stackable")]
        [LabelText("    最大值")]
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
