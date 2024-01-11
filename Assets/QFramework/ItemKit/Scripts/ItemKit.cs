using QFramework.Example;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public class ItemKit
    {
        public static List<Slot> Slots = new List<Slot>();
        public static Dictionary<string, IItem> ItemByKey = new Dictionary<string, IItem>();

        /// <summary>
        /// 通过 Resources 加载指定的物品配置文件。
        /// </summary>
        /// <param name="itemConfigFileName">物品配置文件的名称，不包含路径和扩展名。</param>
        public static void LoadItemConfigByResources(string itemConfigFileName)
        {
            // 使用 Resources.Load 加载名为 itemConfigFileName 的 ItemConfig 对象
            AddItemConfig(Resources.Load<ItemConfig>(itemConfigFileName));
        }

        /// <summary>
        /// 将物品配置添加到物品字典中。
        /// </summary>
        /// <param name="itemConfig">实现了 IItem 接口的物品配置对象。</param>
        public static void AddItemConfig(IItem itemConfig)
        {
            ItemByKey.Add(itemConfig.GetKey, itemConfig);
        }

        /// <summary>
        /// 创建一个新的栏位并添加到栏位列表中。
        /// </summary>
        /// <param name="item">要放入栏位的物品对象，默认为null。</param>
        /// <param name="count">物品的数量，默认为0。</param>
        public static void CreateSlot(IItem item = null, int count = 0)
        {
            Slots.Add(new Slot(item, count));
        }

        /// <summary>
        /// 根据物品的键值查找对应的栏位，该栏位必须已经包含物品且物品数量大于0。
        /// </summary>
        /// <param name="itemKey">物品的键值。</param>
        /// <returns>返回找到的栏位，如果没有找到符合条件的栏位则返回null。</returns>
        public static Slot FindSlotByKey(string itemKey)
        {
            Slot slot = ItemKit.Slots.Find(s => s.Item != null && s.Count > 0 && s.Item.GetKey == itemKey);
            return slot;
        }

        /// <summary>
        /// 查找一个空的栏位，即没有包含任何物品的栏位。
        /// </summary>
        /// <returns>返回找到的空栏位，如果没有空栏位则返回null。</returns>
        public static Slot FindEmptySlot()
        {
            return ItemKit.Slots.Find(s => s.Count == 0);
        }

        /// <summary>
        /// 查找一个可以添加指定物品的栏位。优先查找已包含该物品的栏位，如果没有则查找空栏位。
        /// </summary>
        /// <param name="itemKey">要添加的物品的键值。</param>
        /// <returns>返回可添加物品的栏位，如果没有合适的栏位则返回null。</returns>
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

            if (slot == null || slot.Count >= 99)
            {
                slot.Count = 99;
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
