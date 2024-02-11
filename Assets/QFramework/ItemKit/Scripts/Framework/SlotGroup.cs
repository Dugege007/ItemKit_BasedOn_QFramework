using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QFramework
{
    public class SlotGroup
    {
        public string Key = string.Empty;
        private List<Slot> mSlots = new List<Slot>();
        // 提供一个访问
        public IReadOnlyList<Slot> Slots => mSlots;

        private Func<IItem, bool> mCondition = _ => true;

        public EasyEvent Changed = new EasyEvent();

        public int GetItemCount(IItem item)
        {
            return Slots
                .Where(slot => slot.Count > 0 && slot.Item == item)
                .Sum(slot => slot.Count);
        }

        public SlotGroup CreateSlot(IItem item = null, int count = 0)
        {
            mSlots.Add(new Slot(item, count, this));
            Changed.Trigger();
            return this;
        }

        public SlotGroup CreateSlotsByCount(int count)
        {
            for (int i = 0; i < count; i++)
            {
                CreateSlot();
            }
            Changed.Trigger();
            return this;
        }

        public Slot FindSlotByKey(string itemKey, bool isAdd)
        {
            IItem item = ItemKit.ItemByKey[itemKey];
            Slot slot;

            if (isAdd)
            {
                slot = mSlots.Find(s => s.Item != null && s.Count > 0 && s.Count < item.GetMaxStackableCount && s.Item.GetKey == itemKey);
            }
            else
            {
                if (item.GetStackable)
                {
                    List<Slot> validSlots = mSlots
                        .Where(s => s.Item != null && s.Count > 0 && s.Item.GetKey == itemKey)
                        .OrderBy(s => s.Count) // 按 Count 升序排序
                        .ToList(); // 转换为列表，如果只需要第一个元素，这一步可以省略

                    slot = validSlots.FirstOrDefault(); // 获取Count最小的Slot，如果没有则为null
                }
                else
                {
                    slot = mSlots.Find(s => s.Item != null && s.Count > 0 && s.Item.GetKey == itemKey);
                }
            }

            return slot;
        }

        public Slot FindEmptySlot()
        {
            return mSlots.Find(s => s.Count == 0);
        }

        public Slot FindAddableSlot(string itemKey)
        {
            Slot slot = FindSlotByKey(itemKey, true);
            IItem item = ItemKit.ItemByKey[itemKey];

            if (item.GetStackable) // 如果可堆叠
            {
                if (slot == null || (item.GetHasMaxStackableCount && slot.Count >= item.GetMaxStackableCount))
                {
                    slot = FindEmptySlot();
                    if (slot != null)
                        slot.Item = item;
                }

                return slot;
            }
            else // 不可堆叠
            {
                // 如果有空格子
                slot = FindEmptySlot();
                if (slot != null)
                    slot.Item = item;

                return slot;
            }
        }

        public Slot FindNonFullStackableSlot(string itemKey)
        {
            foreach (Slot slot in mSlots)
            {
                if (slot.Count > 0 && slot.Item.GetKey == itemKey && slot.Count < slot.Item.GetMaxStackableCount)
                    return slot;
            }

            Slot emptySlot = FindEmptySlot();
            if (emptySlot != null)
            {
                emptySlot.Item = ItemKit.ItemByKey[itemKey];
            }
            return emptySlot;
        }

        public struct ItemOperateResult
        {
            public bool Succeed;
            public int RemainCount;
            public MessageTypes MessageTypes;
        }

        public enum MessageTypes
        {
            Full,   // 满了

        }

        // 重载一下
        public ItemOperateResult AddItem(IItem item, int addCount = 1)
        {
            if (item.GetStackable && item.GetHasMaxStackableCount)
            {
                do
                {
                    Slot slot = FindNonFullStackableSlot(item.GetKey);
                    if (slot != null)
                    {
                        int canAddCount = slot.Item.GetMaxStackableCount - slot.Count;
                        if (addCount <= canAddCount)
                        {
                            slot.Count += addCount;
                            slot.Changed.Trigger();
                            Changed.Trigger();
                            return new ItemOperateResult() { Succeed = true, RemainCount = 0, };
                        }
                        else
                        {
                            slot.Count += canAddCount;
                            addCount -= canAddCount;
                            slot.Changed.Trigger();
                            Changed.Trigger();
                        }
                    }
                    else
                    {
                        return new ItemOperateResult() { Succeed = false, RemainCount = addCount };
                    }

                } while (addCount > 0);
            }
            else
            {
                Slot slot = FindAddableSlot(item.GetKey);
                if (slot == null)
                    return new ItemOperateResult() { Succeed = false, RemainCount = addCount };

                slot.Count += addCount;
                slot.Changed.Trigger();
                Changed.Trigger();
            }

            return new ItemOperateResult() { Succeed = true, RemainCount = 0 };
        }


        public ItemOperateResult AddItem(string itemKey, int addCount = 1)
        {
            IItem item = ItemKit.ItemByKey[itemKey];

            return AddItem(item, addCount);
        }

        // 重载一下
        public void RemoveItem(IItem item, int removeCount = 1)
        {
            Slot slot = FindSlotByKey(item.GetKey, false);

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

            slot.Changed.Trigger();
            Changed.Trigger();
        }

        public void RemoveItem(string itemKey, int removeCount = 1)
        {
            Slot slot = FindSlotByKey(itemKey, false);

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

            slot.Changed.Trigger();
        }

        public SlotGroup Condition(Func<IItem, bool> condition)
        {
            mCondition = condition;
            return this;
        }

        public bool CheckCondition(IItem item)
        {
            return mCondition(item);
        }
    }
}
