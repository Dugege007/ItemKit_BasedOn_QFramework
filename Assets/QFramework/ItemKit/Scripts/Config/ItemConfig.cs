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
        [HorizontalGroup("���ͺ�ֵ")]
        [VerticalGroup("���ͺ�ֵ/type",HideWhenChildrenAreInvisible = true)]
        public AttributeType Type;

        [HideLabel]
        [HideIf("Type", AttributeType.Boolean)]
        [VerticalGroup("���ͺ�ֵ/value")]
        public string Value;

        [HideLabel]
        [ShowIf("Type", AttributeType.Boolean)]
        [VerticalGroup("���ͺ�ֵ/value")]
        public bool BoolValue;
    }

    [CreateAssetMenu(menuName = "@ItemKit/Create ItemConfig")]
    public class ItemConfig : ScriptableObject, IItem
    {
        [HideInInspector]
        public ItemDatabase ItemDatabase;

        [HideLabel]
        [PreviewField(48, ObjectFieldAlignment.Left)]
        [HorizontalGroup("��������", 54), VerticalGroup("��������/left")]
        public Sprite Icon = null;

        private void OnValidate()
        {
            this.name = Key;
        }

#if UNITY_EDITOR
        [VerticalGroup("��������/left")]
        [Button("X"), GUIColor(1, 0, 0)]
        private void RemoveThisConfig()
        {
            if (EditorUtility.DisplayDialog("ɾ����Ʒ", "ȷ��Ҫɾ����\n���˲������ɻָ���", "ɾ��", "ȡ��"))
            {
                ItemDatabase.ItemConfigs.Remove(this);
                AssetDatabase.RemoveObjectFromAsset(this);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        [VerticalGroup("��������/left")]
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

        [VerticalGroup("��������/right"), LabelWidth(42)]
        [LabelText("����")]
        public string Name = string.Empty;

        [VerticalGroup("��������/right"), LabelWidth(42)]
        [LabelText("����")]
        [TextArea(minLines: 1, maxLines: 4)]
        public string Description = string.Empty;

        [VerticalGroup("��������/right"), LabelWidth(42)]
        [LabelText("�ؼ���")]
        public string Key = string.Empty;

        [HorizontalGroup("����")]
        [VerticalGroup("����/stackable")]
        [LabelText("����")]
        public List<ItemAttribute> Attributes = new List<ItemAttribute>();

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
