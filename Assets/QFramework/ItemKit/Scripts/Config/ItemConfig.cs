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
        [HorizontalGroup("名称类型", 54), VerticalGroup("名称类型/left")]
        public Sprite Icon = null;

        [VerticalGroup("名称类型/left")]
        [Button]
        public void Refresh()
        {
            this.name = Key;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [VerticalGroup("名称类型/left")]
        [Button, GUIColor(1, 0, 0)]
        private void X()
        {
            if (EditorUtility.DisplayDialog("删除物品", "确定要删除吗？\n（此操作不可恢复）", "删除", "取消"))
            {
                ItemConfigGroup.ItemConfigs.Remove(this);
                AssetDatabase.RemoveObjectFromAsset(this);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
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
        [VerticalGroup("属性/stackable"), LabelWidth(66)]
        [LabelText("可堆叠")]
        public bool IsStackable = true;
        [ShowIf("IsStackable")]
        [VerticalGroup("属性/stackable"), LabelWidth(66)]
        [Indent]
        [LabelText("有最大值")]
        public bool HasMaxStackableCount = false;
        [ShowIf("IsStackable"), EnableIf("HasMaxStackableCount")]
        [DisplayIf(new string[] { "IsStackable", "HasMaxStackableCount" }, new[] { false, false })]
        [VerticalGroup("属性/stackable"), LabelWidth(66)]
        [Indent(2)]
        [LabelText("最大值")]
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
