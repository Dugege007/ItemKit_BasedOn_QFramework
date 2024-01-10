using UnityEngine;
using QFramework;
using System.Collections.Generic;

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

            public Slot(Item item, int count = 1)
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

        private List<Slot> mSlots = new List<Slot>()
        {
            new Slot(new Item("item_1", "物品1")),
            new Slot(new Item("item_2", "物品2")),
            new Slot(new Item("item_3", "物品3")),
            new Slot(new Item("item_4", "物品4")),
        };

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

            if (GUILayout.Button("增加物品1"))
            {
                Slot slot = mSlots.Find(s => s.Item.Key == "item_1");
                slot.Count++;
            }

            if (GUILayout.Button("增加物品2"))
            {
                Slot slot = mSlots.Find(s => s.Item.Key == "item_2");
                slot.Count++;
            }

            if (GUILayout.Button("增加物品3"))
            {
                Slot slot = mSlots.Find(s => s.Item.Key == "item_3");
                slot.Count++;
            }

            if (GUILayout.Button("增加物品4"))
            {
                Slot slot = mSlots.Find(s => s.Item.Key == "item_4");
                slot.Count++;
            }

            if (GUILayout.Button("删除物品1"))
            {
                Slot slot = FindSlotByKey("item_1");
                if (slot != null)
                    slot.Count--;
            }

            if (GUILayout.Button("删除物品2"))
            {
                Slot slot = FindSlotByKey("item_2");
                if (slot != null)
                    slot.Count--;
            }

            if (GUILayout.Button("删除物品3"))
            {
                Slot slot = FindSlotByKey("item_3");
                if (slot != null)
                    slot.Count--;
            }

            if (GUILayout.Button("删除物品4"))
            {
                Slot slot = FindSlotByKey("item_4");
                if (slot != null)
                    slot.Count--;
            }
        }

        private Slot FindSlotByKey(string itemKey)
        {
            Slot slot = mSlots.Find(s => s.Item != null && s.Count > 0 && s.Item.Key == itemKey);
            return slot;
        }
    }
}
