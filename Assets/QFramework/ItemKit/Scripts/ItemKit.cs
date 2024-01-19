using QFramework.Example;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [Serializable]
        public class SlotGroupSaveData
        {
            public string Key;
            public List<SlotSaveData> SlotsSaveDatas = new List<SlotSaveData>();
        }

        [Serializable]
        public class SlotSaveData
        {
            public string ItemKey;
            public int Count;
        }

        public static void Save()
        {
            foreach (var slopGroup in mSlotGroupByKey.Values)
            {
                SlotGroupSaveData slotGroupSaveData = new()
                {
                    Key = slopGroup.Key,
                    SlotsSaveDatas = slopGroup.Slots.Select(slot => new SlotSaveData()
                    {
                        ItemKey = slot.Item != null ? slot.Item.GetKey : null,
                        Count = slot.Count,
                    }).ToList(),
                };

                string json = JsonUtility.ToJson(slotGroupSaveData);
                json.LogInfo();
            }

            Debug.Log("保存数据");
        }

        public static void Load()
        {
            Debug.Log("加载数据");
        }
    }
}
