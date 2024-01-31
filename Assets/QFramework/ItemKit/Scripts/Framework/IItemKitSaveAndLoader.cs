using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QFramework
{
    public interface IItemKitSaveAndLoader
    {
        void Save(Dictionary<string, SlotGroup> slotGroups);
        void Load(Dictionary<string, SlotGroup> slotGroups);
        void Clear();
    }

    public class DefaultItemKitSaverAndLoader : IItemKitSaveAndLoader
    {
        [Serializable]
        public class SaveData
        {
            public string Key;
            public List<SlotData> SlotsSaveDatas = new List<SlotData>();
        }

        [Serializable]
        public class SlotData
        {
            public string ItemKey;
            public int Count;
        }

        public void Save(Dictionary<string, SlotGroup> slotGroups)
        {
            var keyString = string.Join("@@", slotGroups.Keys.ToList());
            PlayerPrefs.SetString("slot_group_keys", keyString);

            foreach (var slotGroup in slotGroups.Values)
            {
                SaveData slotGroupSaveData = new()
                {
                    Key = slotGroup.Key,
                    SlotsSaveDatas = slotGroup.Slots.Select(slot => new SlotData()
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

        public void Load(Dictionary<string, SlotGroup> slotGroups)
        {
            string keyString = PlayerPrefs.GetString("slot_group_keys", string.Empty);
            List<string> keys = keyString.Split("@@").ToList();

            foreach (var key in keys)
            {
                SlotGroup slotGroup;

                if (slotGroups.ContainsKey(key))
                    slotGroup = slotGroups[key];
                else
                    slotGroup = ItemKit.CreateSlotGroup(key);

                var json = PlayerPrefs.GetString("slot_group_" + slotGroup.Key, string.Empty);

                if (json.IsNullOrEmpty())
                {

                }
                else
                {
                    SaveData saveData = JsonUtility.FromJson<SaveData>(json);
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

        public void Clear()
        {
            string keyString = PlayerPrefs.GetString("slot_group_keys", string.Empty);
            List<string> keys = keyString.Split("@@").ToList();

            foreach (var key in keys)
            {
                PlayerPrefs.DeleteKey("slot_group_" + key);
            }

            PlayerPrefs.DeleteKey("slot_group_keys");
        }
    }
}
