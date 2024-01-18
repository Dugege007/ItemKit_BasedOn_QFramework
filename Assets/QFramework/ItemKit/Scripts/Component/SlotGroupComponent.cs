using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public class SlotGroupComponent : MonoBehaviour
    {
        [Serializable]
        public class SlotConfig
        {
            public ItemConfig Item;
            public int Count;
        }

        public string GroupKey;
        [Header("初始栏位")]
        public List<SlotConfig> InitSlots = new List<SlotConfig>();
        [DisplayLabel("关联的 UISlotGroup")]
        public UISlotGroup UISlotGroup;

        private void Awake()
        {
            SlotGroup group = ItemKit.CreateSlotGroup(GroupKey);
            foreach (var slotConfig in InitSlots)
            {
                group.CreateSlot(slotConfig.Item, slotConfig.Count);
            }
        }

        public void Open()
        {
            UISlotGroup.RefreshWithChangeGroupKey(GroupKey);
            UISlotGroup.Show();
        }

        public void Close()
        {
            UISlotGroup.Hide();
        }
    }
}
