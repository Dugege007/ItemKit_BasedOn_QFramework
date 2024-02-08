using System;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public class SlotGroupInScene : MonoBehaviour
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
            // 检查是否已经存在同名的槽位组
            if (ItemKit.HasSlotGroup(GroupKey) == false)
            {
                // 如果不存在同名的槽位组，则创建新的槽位组
                SlotGroup group = ItemKit.CreateSlotGroup(GroupKey);
                foreach (var slotConfig in InitSlots)
                {
                    group.CreateSlot(slotConfig.Item, slotConfig.Count);
                }
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
