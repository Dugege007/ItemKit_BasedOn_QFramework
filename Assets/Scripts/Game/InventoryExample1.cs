using UnityEngine;
using QFramework;
using System.Collections.Generic;
using static QFramework.Example.InventoryExample1;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace QFramework.Example
{
    public partial class InventoryExample1 : ViewController
    {
        private void Start()
        {
            // Code Here
        }

        public class Slot
        {
            public Item Item;
            public int Count;

            public Slot(Item item, int count = 0)
            {
                Item = item;
                Count = count;
            }
        }

        public class Item
        {
            public string Key;  // 或者是ID，用于查找
            public string Name;

            public Item(string key, string name)
            {
                Key = key;
                Name = name;
            }
        }

        public Item Item1 = new Item("item_1", "物品1");
        public Item Item2 = new Item("item_2", "物品2");
        public Item Item3 = new Item("item_3", "物品3");
        public Item Item4 = new Item("item_4", "物品4");
        public Item Item5 = new Item("item_5", "物品5");

        private List<Slot> mSlots = null;

        private Dictionary<string, Item> mItemByKey = null;

        private void Awake()
        {
            mSlots = new List<Slot>()
            {
                new Slot(Item1, 1),
                new Slot(Item2, 2),
                new Slot(Item3, 3),
                new Slot(Item4, 4),
            };

            mItemByKey = new Dictionary<string, Item>()
            {
                { Item1.Key, Item1 },
                { Item2.Key, Item2 },
                { Item3.Key, Item3 },
                { Item4.Key, Item4 },
            };
        }

        private void OnGUI()
        {
            // 先规定一下分辨率
            IMGUIHelper.SetDesignResolution(960, 540);

            foreach (var slot in mSlots)
            {
                GUILayout.BeginHorizontal("box");
                if (slot.Count <= 0)
                    GUILayout.Label($"格子：空");
                else
                    GUILayout.Label($"格子：{slot.Item.Name} x {slot.Count}");
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label(Item1.Name);
            if (GUILayout.Button("+")) AddItem(Item1.Key);
            if (GUILayout.Button("-")) RemoveItem(Item1.Key);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(Item2.Name);
            if (GUILayout.Button("+")) AddItem(Item2.Key);
            if (GUILayout.Button("-")) RemoveItem(Item2.Key);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(Item3.Name);
            if (GUILayout.Button("+")) AddItem(Item3.Key);
            if (GUILayout.Button("-")) RemoveItem(Item3.Key);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(Item4.Name);
            if (GUILayout.Button("+")) AddItem(Item4.Key);
            if (GUILayout.Button("-")) RemoveItem(Item4.Key);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(Item5.Name);
            if (GUILayout.Button("+")) AddItem(Item5.Key);
            if (GUILayout.Button("-")) RemoveItem(Item5.Key);
            GUILayout.EndHorizontal();
        }

        private Slot FindSlotByKey(string itemKey)
        {
            Slot slot = mSlots.Find(s => s.Item != null && s.Count > 0 && s.Item.Key == itemKey);
            return slot;
        }

        private Slot FindEmptySlot()
        {
            return mSlots.Find(s => s.Count == 0);
        }

        private Slot FindAddableSlot(string itemKey)
        {
            Slot slot = FindSlotByKey(itemKey);

            if (slot == null)
            {
                slot = FindEmptySlot();
                if (slot != null)
                    slot.Item = mItemByKey[itemKey];
            }

            return slot;
        }

        private void AddItem(string itemKey, int addCount = 1)
        {
            Slot slot = FindAddableSlot(itemKey);

            if (slot == null)
            {
                Debug.Log("背包满了");
                return;
            }

            slot.Count += addCount;
        }

        private void RemoveItem(string itemKey, int removeCount = 1)
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
