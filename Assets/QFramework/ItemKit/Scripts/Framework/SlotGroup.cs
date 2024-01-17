using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public class SlotGroup
    {
        public string Key = string.Empty;
        private List<Slot> mSlots = new List<Slot>();

        // 提供一个访问
        public IReadOnlyList<Slot> Slots => mSlots;

        public SlotGroup CreateSlot(IItem item = null, int count = 0)
        {
            mSlots.Add(new Slot(item, count));
            return this;
        }

        public SlotGroup CreateSlotsByCount(int count)
        {
            for (int i = 0; i < count; i++)
            {
                CreateSlot();
            }
            return this;
        }

        public Slot FindSlotByKey(string itemKey)
        {
            Slot slot = mSlots.Find(s => s.Item != null && s.Count > 0 && s.Item.GetKey == itemKey);
            return slot;
        }

        public Slot FindEmptySlot()
        {
            return mSlots.Find(s => s.Count == 0);
        }

        public Slot FindAddableSlot(string itemKey)
        {
            Slot slot = FindSlotByKey(itemKey);

            if (slot == null)
            {
                slot = FindEmptySlot();
                if (slot != null)
                    slot.Item = ItemKit.ItemByKey[itemKey];
            }

            return slot;
        }

        public void AddItem(string itemKey, int addCount = 1)
        {
            Slot slot = FindAddableSlot(itemKey);

            if (slot == null || slot.Count >= 99)
            {
                slot.Count = 99;
                Debug.Log("背包满了");
                return;
            }

            slot.Count += addCount;
        }

        public void RemoveItem(string itemKey, int removeCount = 1)
        {
            Slot slot = FindSlotByKey(itemKey);

            if (slot == null || slot.Count < 1)
            {
                Debug.Log("背包中没有此物品");
                return;
            }

            if (slot.Count < removeCount)
            {
                Debug.Log("物品不足");
                return;
            }

            slot.Count -= removeCount;

            if (slot.Count <= 0)
            {
                slot.Count = 0;
                // 当数量减到0时，清除栏位中的物品引用
                slot.Item = null;
            }
        }
    }
}
