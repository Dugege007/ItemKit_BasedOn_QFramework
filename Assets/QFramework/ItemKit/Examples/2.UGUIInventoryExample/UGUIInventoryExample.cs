using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace QFramework.Example
{
    public partial class UGUIInventoryExample : ViewController
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

                Debug.Log("加载数据：" + json);
            }

            public void Clear()
            {
                PlayerPrefs.DeleteKey("my_item_kit");
            }
        }

        private void Awake()
        {
            ItemKit.SaverAndLoader = new MySaverAndLoader();

            ItemKit.LoadItemDatabase("ExampleItemConfigGroup");

            ItemKit.CreateSlotGroup("物品栏")
                .CreateSlot(ItemKit.ItemByKey[Items.item_iron], 1)
                .CreateSlot(ItemKit.ItemByKey[Items.item_green_sword], 1)
                .CreateSlotsByCount(6);

            ItemKit.CreateSlotGroup("背包")
                .CreateSlotsByCount(20);

            ItemKit.CreateSlotGroup("宝箱")
                .CreateSlotsByCount(10);

            ItemKit.CreateSlotGroup("宝箱2")
                .CreateSlotsByCount(5);

            ItemKit.CreateSlotGroup("武器")
                .CreateSlot(null, 0)
                .Condition(item => item.GetBoolean("IsWeapon"));

            ItemKit.Load();

            Slot weaponSlot = ItemKit.GetSlotGroupByKey("武器").Slots[0];
            weaponSlot.Changed.Register(() =>
            {
                if (weaponSlot.Count > 0)
                    Debug.Log("已切换武器为：" + weaponSlot.Item.GetName);

            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void Start()
        {
            TreasureBoxExample.Hide();

            #region 添加物品
            BtnAddItem1.onClick.AddListener(() =>
            {
                SlotGroup.ItemOperateResult result = ItemKit.GetSlotGroupByKey("物品栏") // 先拿到背包
                    .AddItem(Items.item_iron, 20);    // 再添加物品

                Debug.Log("剩余未添加物品的数量：" + result.RemainCount);
                if (!result.Succeed)
                {
                    if (result.MessageTypes == SlotGroup.MessageTypes.Full)
                        Debug.Log("背包满了");
                }
            });

            BtnAddItem2.onClick.AddListener(() =>
            {
                SlotGroup.ItemOperateResult result = ItemKit.GetSlotGroupByKey("物品栏")
                    .AddItem(Items.item_green_sword);

                if (!result.Succeed)
                {
                    if (result.MessageTypes == SlotGroup.MessageTypes.Full)
                        Debug.Log("背包满了");
                }
            });

            BtnAddItem3.onClick.AddListener(() =>
            {
                SlotGroup.ItemOperateResult result = ItemKit.GetSlotGroupByKey("物品栏")
                    .AddItem(Items.item_paper);

                if (!result.Succeed)
                {
                    if (result.MessageTypes == SlotGroup.MessageTypes.Full)
                        Debug.Log("背包满了");
                }
            });
            #endregion

            #region 删除物品
            BtnRemoveItem1.onClick.AddListener(() =>
            {
                ItemKit.GetSlotGroupByKey("物品栏")
                    .RemoveItem(Items.item_iron);
            });

            BtnRemoveItem2.onClick.AddListener(() =>
            {
                ItemKit.GetSlotGroupByKey("物品栏")
                    .RemoveItem(Items.item_green_sword);
            });

            BtnRemoveItem3.onClick.AddListener(() =>
            {
                ItemKit.GetSlotGroupByKey("物品栏")
                    .RemoveItem(Items.item_paper);
            });
            #endregion

            BtnTreasureBox.onClick.AddListener(() =>
            {
                UISlotGroup group = TreasureBoxExample.GetComponent<UISlotGroup>();
                group.RefreshWithChangeGroupKey("宝箱");
            });

            BtnTreasureBox2.onClick.AddListener(() =>
            {
                UISlotGroup group = TreasureBoxExample.GetComponent<UISlotGroup>();
                group.RefreshWithChangeGroupKey("宝箱2");
            });
        }

        private void OnApplicationQuit()
        {
            ItemKit.Save();
        }
    }
}
