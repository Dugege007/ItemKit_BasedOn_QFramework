using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace QFramework.Example
{
    public class MySaverAndLoader : IItemKitSaveAndLoader
    {
        [Serializable]
        public class SaveData
        {
            public List<GroupData> GroupDatas = new List<GroupData>();
        }

        [Serializable]
        public class GroupData
        {
            public string Key;
            public List<SlotData> SlotDatas = new List<SlotData>();
        }

        [Serializable]
        public class SlotData
        {
            public string ItemKey;
            public int Count;
        }

        public void Save(Dictionary<string, SlotGroup> slotGroups)
        {
            PlayerPrefs.SetString("my_item_kit", JsonUtility.ToJson(new SaveData()
            {
                GroupDatas = slotGroups.Values.Select(group => new GroupData()
                {
                    Key = group.Key,
                    SlotDatas = group.Slots.Select(slot => new SlotData()
                    {
                        ItemKey = slot.Item?.GetKey,
                        Count = slot.Count,
                    }).ToList()
                }).ToList()
            }));

            Debug.Log("保存数据");
        }

        public void Load(Dictionary<string, SlotGroup> slotGroups)
        {
            string json = PlayerPrefs.GetString("my_item_kit", string.Empty);

            if (json.IsNotNullAndEmpty())
            {
                SaveData saveData = JsonUtility.FromJson<SaveData>(json);

                foreach (var group in saveData.GroupDatas)
                {
                    SlotGroup slotGroup;
                    if (slotGroups.ContainsKey(group.Key))
                        slotGroup = slotGroups[group.Key];
                    else
                        slotGroup = ItemKit.CreateSlotGroup(group.Key);

                    for (int i = 0; i < group.SlotDatas.Count; i++)
                    {
                        var slotSaveData = group.SlotDatas[i];
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
            PlayerPrefs.DeleteKey("my_item_kit");
        }
    }
}
