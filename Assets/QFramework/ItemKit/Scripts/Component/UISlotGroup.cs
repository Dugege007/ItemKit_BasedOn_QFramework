using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public class UISlotGroup : MonoBehaviour
    {
        public enum UISlotGenerateType
        {
            GenerateByTemplate,
            UseExistUISlot,
        }

        public string GroupKey;

        [DisplayLabel("UISlot 生成类型")]
        public UISlotGenerateType Type;

        [DisplayIf(nameof(Type), false, UISlotGenerateType.GenerateByTemplate)]
        public UISlot UISlotTemplate;
        [DisplayIf(nameof(Type), false, UISlotGenerateType.GenerateByTemplate)]
        public RectTransform UISlotRoot;

        [DisplayIf(nameof(Type), false, UISlotGenerateType.UseExistUISlot)]
        public List<UISlot> UISlots;

        private void Start()
        {
            Refresh();
        }

        public void RefreshWithChangeGroupKey(string groupKey)
        {
            if (GroupKey != groupKey)
                gameObject.SetActive(true);
            else
                gameObject.SetActive(!gameObject.activeSelf);

            GroupKey = groupKey;
            Refresh();
        }

        public void Refresh()
        {
            if (Type == UISlotGenerateType.GenerateByTemplate)
            {
                UISlotTemplate.Hide();

                UISlotRoot.DestroyChildrenWithCondition(child => child.GetComponent<UISlot>());

                foreach (var slot in ItemKit.GetSlotGroupByKey(GroupKey).Slots)
                {
                    UISlotTemplate.InstantiateWithParent(UISlotRoot)
                        .InitWithData(slot)
                        .Show();
                }
            }
            else if (Type == UISlotGenerateType.UseExistUISlot)
            {
                var slotGroup = ItemKit.GetSlotGroupByKey(GroupKey);
                var slots = slotGroup.Slots;

                if (slots != null && UISlots.Count <= slots.Count)
                {
                    for (int i = 0; i < UISlots.Count; i++)
                    {
                        UISlots[i].InitWithData(slots[i]);
                    }
                }
                else
                {
                    Debug.Log("UISlots 的数量大于 SlotGroup 中的 Slots 数量或 SlotGroup 为 null");
                }
            }
        }
    }
}
