using QFramework.Example;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QFramework
{
    public class ItemKit
    {
        public static IItemKitSaveAndLoader SaverAndLoader;

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
            ItemConfigGroup database = Resources.Load<ItemConfigGroup>(databaseName);
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
            foreach (var slotGroup in mSlotGroupByKey.Values)
            {
                SlotGroupSaveData slotGroupSaveData = new()
                {
                    Key = slotGroup.Key,
                    SlotsSaveDatas = slotGroup.Slots.Select(slot => new SlotSaveData()
                    {
                        ItemKey = slot.Item?.GetKey,
                        Count = slot.Count,
                    }).ToList(),
                };

                string json = JsonUtility.ToJson(slotGroupSaveData);
                PlayerPrefs.SetString("slot_group_" + slotGroup.Key, json);
            }

            Debug.Log("保存数据");
        }

        public static void Load()
        {
            foreach (var slotGroup in mSlotGroupByKey.Values)
            {
                var json = PlayerPrefs.GetString("slot_group_" + slotGroup.Key, string.Empty);

                if (json.IsNullOrEmpty())
                {

                }
                else
                {
                    SlotGroupSaveData saveData = JsonUtility.FromJson<SlotGroupSaveData>(json);
                    for (int i = 0; i < saveData.SlotsSaveDatas.Count; i++)
                    {
                        var slotSaveData = saveData.SlotsSaveDatas[i];
                        var item = slotSaveData.ItemKey.IsNullOrEmpty()
                                ? null
                                : ItemKit.ItemByKey[slotSaveData.ItemKey];

                        if (i < slotGroup.Slots.Count)
                        {
                            slotGroup.Slots[i].Item = item;
                            slotGroup.Slots[i].Count = slotSaveData.Count;
                            slotGroup.Slots[i].Changed.Trigger();
                        }
                        else
                        {
                            slotGroup.CreateSlot(item, slotSaveData.Count);
                        }
                    }
                }
            }

            Debug.Log("加载数据");
        }
    }
}
