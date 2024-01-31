using QFramework.Example;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QFramework
{
    public class ItemKit
    {
        public static IItemKitSaveAndLoader SaverAndLoader = new DefaultItemKitSaverAndLoader();

        public static Dictionary<string, SlotGroup> mSlotGroupByKey = new Dictionary<string, SlotGroup>();
        public static Dictionary<string, IItem> ItemByKey = new Dictionary<string, IItem>();

        public static UISlot CurrentSlotPointerOn = null;

        public static SlotGroup CreateSlotGroup(string key)
        {
            SlotGroup slotGroup = new SlotGroup()
            {
                Key = key,
            };

            if (!mSlotGroupByKey.ContainsKey(key))
            {
                mSlotGroupByKey.Add(key, slotGroup);
            }
            return slotGroup;
        }

        public static SlotGroup GetSlotGroupByKey(string key)
        {
            return mSlotGroupByKey[key];
        }

        public static void LoadItemDatabase(string databaseName)
        {
            ItemConfigGroup database = Resources.Load<ItemConfigGroup>(databaseName);
            database.ItemConfigs.ForEach(config => AddItemConfig(config));
        }

        public static void AddItemConfig(IItem itemConfig)
        {
            ItemByKey.Add(itemConfig.GetKey, itemConfig);
        }

        public static void Save() => SaverAndLoader.Save(mSlotGroupByKey);

        public static void Load() => SaverAndLoader.Load(mSlotGroupByKey);

        public static void Clear() => SaverAndLoader.Clear();
    }
}
