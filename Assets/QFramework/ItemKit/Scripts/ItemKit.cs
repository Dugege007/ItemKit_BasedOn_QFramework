using QFramework.Example;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public class ItemKit
    {
        public static Dictionary<string, SlotGroup> mSlotGroupByKey = new Dictionary<string, SlotGroup>();
        public static Dictionary<string, IItem> ItemByKey = new Dictionary<string, IItem>();

        public static UISlot CurrentSlotPointerOn = null;

        public static SlotGroup CreateSlotGroup(string key)
        {
            SlotGroup slotGroup = new SlotGroup()
            {
                Key = key,
            };

            mSlotGroupByKey.Add(key, slotGroup);
            return slotGroup;
        }

        public static SlotGroup GetSlotGroupByKey(string key)
        {
            return mSlotGroupByKey[key];
        }

        public static void LoadItemDatabase(string databaseName)
        {
            ItemDatabase database = Resources.Load<ItemDatabase>(databaseName);
            database.ItemConfigs.ForEach(config => AddItemConfig(config));
        }

        public static void AddItemConfig(IItem itemConfig)
        {
            ItemByKey.Add(itemConfig.GetKey, itemConfig);
        }
    }
}
