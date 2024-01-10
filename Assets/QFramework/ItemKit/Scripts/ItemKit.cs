using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public class ItemKit
    {
        public static Item Item1 = new Item("item_1", "物品1");
        public static Item Item2 = new Item("item_2", "物品2");
        public static Item Item3 = new Item("item_3", "物品3");
        public static Item Item4 = new Item("item_4", "物品4");
        public static Item Item5 = new Item("item_5", "物品5");

        public static List<Slot> Slots = new List<Slot>()
        {
            new Slot(ItemKit.Item1, 1),
            new Slot(ItemKit.Item2, 2),
            new Slot(ItemKit.Item3, 3),
            new Slot(ItemKit.Item4, 4),
        };

        public static Dictionary<string, Item> ItemByKey = new Dictionary<string, Item>()
        {
            { ItemKit.Item1.Key, ItemKit.Item1 },
            { ItemKit.Item2.Key, ItemKit.Item2 },
            { ItemKit.Item3.Key, ItemKit.Item3 },
            { ItemKit.Item4.Key, ItemKit.Item4 },
        };

        /// <summary>
        /// 根据物品的键值查找对应的槽位，该槽位必须已经包含物品且物品数量大于0。
        /// </summary>
        /// <param name="itemKey">物品的键值。</param>
        /// <returns>返回找到的槽位，如果没有找到符合条件的槽位则返回null。</returns>
        public static Slot FindSlotByKey(string itemKey)
        {
            Slot slot = ItemKit.Slots.Find(s => s.Item != null && s.Count > 0 && s.Item.Key == itemKey);
            return slot;
        }

        /// <summary>
        /// 查找一个空的槽位，即没有包含任何物品的槽位。
        /// </summary>
        /// <returns>返回找到的空槽位，如果没有空槽位则返回null。</returns>
        public static Slot FindEmptySlot()
        {
            return ItemKit.Slots.Find(s => s.Count == 0);
        }

        /// <summary>
        /// 查找一个可以添加指定物品的槽位。优先查找已包含该物品的槽位，如果没有则查找空槽位。
        /// </summary>
        /// <param name="itemKey">要添加的物品的键值。</param>
        /// <returns>返回可添加物品的槽位，如果没有合适的槽位则返回null。</returns>
        public static Slot FindAddableSlot(string itemKey)
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

        /// <summary>
        /// 向背包中添加指定键值的物品。
        /// </summary>
        /// <param name="itemKey">要添加的物品的键值。</param>
        /// <param name="addCount">要添加的物品数量，默认为1。</param>
        public static void AddItem(string itemKey, int addCount = 1)
        {
            Slot slot = FindAddableSlot(itemKey);

            if (slot == null)
            {
                Debug.Log("背包满了");
                return;
            }

            slot.Count += addCount;
        }

        /// <summary>
        /// 从背包中移除指定键值的物品。
        /// </summary>
        /// <param name="itemKey">要移除的物品的键值。</param>
        /// <param name="removeCount">要移除的物品数量，默认为1。</param>
        public static void RemoveItem(string itemKey, int removeCount = 1)
        {
            Slot slot = FindSlotByKey(itemKey);

            if (slot == null || slot.Count < 1)
            {
                Debug.Log("背包中没有此物品");
                return;
            }

            slot.Count -= removeCount;

            if (slot.Count == 0)
            {
                // 当数量减到0时，清除槽位中的物品引用
                slot.Item = null;
            }
        }
    }
}
