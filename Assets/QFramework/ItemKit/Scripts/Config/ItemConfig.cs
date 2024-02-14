using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    [Serializable]
    public class ItemAttribute
    {
        [HideLabel, LabelWidth(42)]
        public string Name;

        [HideLabel]
        [HorizontalGroup("类型和值")]
        [VerticalGroup("类型和值/type",HideWhenChildrenAreInvisible = true)]
        public AttributeType Type;

        [HideLabel]
        [HideIf("Type", AttributeType.Boolean)]
        [VerticalGroup("类型和值/value")]
        public string Value;

        [HideLabel]
        [ShowIf("Type", AttributeType.Boolean)]
        [VerticalGroup("类型和值/value")]
        public bool BoolValue;
    }

    [CreateAssetMenu(menuName = "@ItemKit/Create ItemConfig")]
    public class ItemConfig : ScriptableObject, IItem
    {
        [HideInInspector]
        public ItemDatabase ItemDatabase;

        [HideLabel]
        [PreviewField(48, ObjectFieldAlignment.Left)]
        [HorizontalGroup("名称类型", 54), VerticalGroup("名称类型/left")]
        public Sprite Icon = null;

        private void OnValidate()
        {
            this.name = Key;
        }

#if UNITY_EDITOR
        [VerticalGroup("名称类型/left")]
        [Button("X"), GUIColor(1, 0, 0)]
        private void RemoveThisConfig()
        {
            if (EditorUtility.DisplayDialog("删除物品", "确定要删除吗？\n（此操作不可恢复）", "删除", "取消"))
            {
                ItemDatabase.ItemConfigs.Remove(this);
                AssetDatabase.RemoveObjectFromAsset(this);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        [VerticalGroup("名称类型/left")]
        [Button("Dup"), GUIColor("yellow")]
        private void DuplicateThisConfig()
        {
            if (ItemDatabase == null)
            {
                Debug.LogError("ItemConfigGroup is null!");
                return;
            }
            ItemDatabase.DuplicateItemConfig(ItemDatabase.ItemConfigs.IndexOf(this), this);
        }
#endif

        [VerticalGroup("名称类型/right"), LabelWidth(42)]
        [LabelText("名称")]
        public string Name = string.Empty;

        [VerticalGroup("名称类型/right"), LabelWidth(42)]
        [LabelText("描述")]
        [TextArea(minLines: 1, maxLines: 4)]
        public string Description = string.Empty;

        [VerticalGroup("名称类型/right"), LabelWidth(42)]
        [LabelText("关键字")]
        public string Key = string.Empty;

        [HorizontalGroup("属性")]
        [VerticalGroup("属性/stackable")]
        [LabelText("属性")]
        public List<ItemAttribute> Attributes = new List<ItemAttribute>();

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

        public string GetName => ItemKit.CurrentLanguage == ItemKit.DefaultLanguage ? Name : LocalItem.Name;
        public string GetKey => Key;
        public string GetDescription => ItemKit.CurrentLanguage == ItemKit.DefaultLanguage ? Description : LocalItem.Description;
        public Sprite GetIcon => Icon;
        public bool GetStackable => IsStackable;
        public bool GetHasMaxStackableCount => HasMaxStackableCount;
        public int GetMaxStackableCount => MaxStackableCount;
        public ItemLanguagePackage.LocalItem LocalItem { get; set; }

        public bool GetBoolean(string attributeName)
        {
            ItemAttribute attribute = Attributes.FirstOrDefault(attribute => attribute.Name == attributeName);

            return attribute.BoolValue;
        }

        public int GetInt(string attributeName)
        {
            ItemAttribute attribute = Attributes.FirstOrDefault(attribute => attribute.Name == attributeName);

            if (int.TryParse(attribute.Value, out var result))
                return result;

            return 0;
        }

        public float GetFloat(string attributeName)
        {
            ItemAttribute attribute = Attributes.FirstOrDefault(attribute => attribute.Name == attributeName);

            if (float.TryParse(attribute.Value, out var result))
                return result;

            return 0;
        }

        public string GetString(string attributeName)
        {
            ItemAttribute attribute = Attributes.FirstOrDefault(attribute => attribute.Name == attributeName);

            return attribute.Value;
        }
    }
}
