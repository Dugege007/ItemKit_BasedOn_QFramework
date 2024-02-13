using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.U2D;
using System.Linq;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace QFramework.Example
{
    public partial class UGUIInventoryExample : ViewController
    {
        // 测试用
        //public class MyItemKitLoader : IItemKitLoader
        //{
        //    // 用 QFramework 的 ResLoader
        //    public ResLoader ResLoader { get; set; }

        //    public ItemConfigGroup LoadItemDatabase(string databaseName)
        //    {
        //        // 加载 Resources 目录下的内容，要加一个 "resources://"
        //        return ResLoader.LoadSync<ItemConfigGroup>("resources://" + databaseName);

        //        // 加载 AssetBundle 的内容，可以直接写 databaseName
        //        // 加载 AssetBundle 的内容，需要将相应的资源进行标记
        //        //return ResLoader.LoadSync<ItemConfigGroup>(databaseName);
        //    }

        //    public void LoadItemDatabaseAsync(string databaseName, Action<ItemConfigGroup> onLoadFinish)
        //    {
        //    }

        //    public ItemLanguagePackage LoadLanguagePackage(string languagePackageName)
        //    {
        //        return ResLoader.LoadSync<ItemLanguagePackage>("resources://" + languagePackageName);
        //    }

        //    public void LoadLanguagePackageAsync(string languagePackageName, Action<ItemLanguagePackage> onLoadFinish)
        //    {
        //    }
        //}

        private static int mIronCount = 10;

        public class MyExcelItem : IItem
        {
            public string GetKey { get; set; }

            public string GetName { get; set; }

            public string GetDescription { get; set; }

            public Sprite GetIcon { get; set; }

            public List<ItemAttribute> Attributes = new List<ItemAttribute>();

            public bool GetStackable { get; set; }

            public bool GetHasMaxStackableCount { get; set; }

            public int GetMaxStackableCount { get; set; }

            public ItemLanguagePackage.LocalItem LocalItem { get; set; }

            public bool GetBoolean(string attributeName)
            {
                ItemAttribute attribute = Attributes.FirstOrDefault(attribute => attribute.Name == attributeName);

                return attribute.BoolValue;
            }

            public int GetInt(string attributeName)
            {
                ItemAttribute attribute = Attributes.FirstOrDefault(attribute => attribute.Name == attributeName);

                if (int.TryParse(attribute.Value, out var result))
                    return result;

                return 0;
            }

            public float GetFloat(string attributeName)
            {
                ItemAttribute attribute = Attributes.FirstOrDefault(attribute => attribute.Name == attributeName);

                if (float.TryParse(attribute.Value, out var result))
                    return result;

                return 0;
            }

            public string GetString(string attributeName)
            {
                ItemAttribute attribute = Attributes.FirstOrDefault(attribute => attribute.Name == attributeName);

                return attribute.Value;
            }

            public bool IsWeapon { get; set; }
        }

        private void Awake()
        {
            // 读取 CSV 文件
            TextAsset itemTextAsset = Resources.Load<TextAsset>("Items");
            SpriteAtlas iconAtlas = Resources.Load<SpriteAtlas>("IconAtlas");
            string itemString = itemTextAsset.text;

            string[] itemRows = itemString.Split("\n");
            int line = 0;
            // key	name	desctiption	icon	stackable	max_count	is_weapon
            List<MyExcelItem> excelItems = new List<MyExcelItem>();
            foreach (string row in itemRows)
            {
                if (line > 0)
                {
                    if (row.IsTrimNotNullAndEmpty())
                    {
                        string[] itemFields = row.Split(",");
                        string key = itemFields[0];
                        string name = itemFields[1];
                        string description = itemFields[2];
                        string icon = itemFields[3];
                        string stackable = itemFields[4];
                        string maxCount = itemFields[5];
                        string isWeapon = itemFields[6];

                        MyExcelItem item = new MyExcelItem()
                        {
                            GetKey = key,
                            GetName = name,
                            GetIcon = iconAtlas.GetSprite(icon),
                            GetDescription = description,
                            GetStackable = int.Parse(stackable) == 1,
                            GetMaxStackableCount = int.Parse(maxCount),
                            IsWeapon = int.Parse(isWeapon) == 1,
                        };

                        excelItems.Add(item);
                    }
                }

                line++;
            }

            itemString.LogInfo();

            //ResKit.Init();
            //ResLoader resLoader = ResLoader.Allocate();

            // 将 Loader 替换
            ItemKit.SaverAndLoader = new MySaverAndLoader();
            //ItemKit.Loader = new MyItemKitLoader()
            //{
            //    ResLoader = resLoader,
            //};

            ItemKit.LoadItemDatabase("ExampleItemDatabase");
            ItemKit.LoadItemLanguagePackage("ItemEnglishPackage");

            foreach (var item in excelItems)
                ItemKit.ItemByKey.Add(item.GetKey, item);

            ItemKit.CreateSlotGroup("物品栏")
                .CreateSlot(Items.item_iron, 1)
                .CreateSlot(Items.item_green_sword, 1)
                .CreateSlotByKey("iron_sword", 1)
                .CreateSlotByKey("shoe", 1)
                .CreateSlotsByCount(6);

            ItemKit.CreateSlotGroup("背包")
                .CreateSlotsByCount(20);

            ItemKit.CreateSlotGroup("宝箱")
                .CreateSlotsByCount(10);

            ItemKit.CreateSlotGroup("宝箱2")
                .CreateSlotsByCount(5);

            ItemKit.CreateSlotGroup("武器")
                .CreateSlot(null, 0)
                .Condition(item => item.GetBoolean("是武器")); // 添加限制条件：如果是武器，才可以拖入

            ItemKit.CreateSlotGroup("商店")
                .Condition(_ => false); // 添加限制条件：不可拖动

            ItemKit.CreateSlotGroup("锻造")
                .Condition(_ => false); // 添加限制条件：不可拖动

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
                    .AddItem(Items.item_iron_key, 15);    // 再添加物品

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
                    .AddItem(Items.item_green_sword_key);

                if (!result.Succeed)
                {
                    if (result.MessageTypes == SlotGroup.MessageTypes.Full)
                        Debug.Log("背包满了");
                }
            });

            BtnAddItem3.onClick.AddListener(() =>
            {
                SlotGroup.ItemOperateResult result = ItemKit.GetSlotGroupByKey("物品栏")
                    .AddItem(Items.item_paper_key);

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
                    .RemoveItem(Items.item_iron_key);
            });

            BtnRemoveItem2.onClick.AddListener(() =>
            {
                ItemKit.GetSlotGroupByKey("物品栏")
                    .RemoveItem(Items.item_green_sword_key);
            });

            BtnRemoveItem3.onClick.AddListener(() =>
            {
                ItemKit.GetSlotGroupByKey("物品栏")
                    .RemoveItem(Items.item_paper_key);
            });
            #endregion

            // 宝箱
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

            // 商店
            BtnShop1.onClick.AddListener(() =>
            {
                ShopConfig shopConfig = BtnShop1.GetComponent<ShopConfig>();
                UIShop.Title.text = shopConfig.ShopName;

                // 出售
                ShopSellTable sellTable = new ShopSellTable();
                foreach (var sellItem in shopConfig.SellItems)
                    sellTable.Add(sellItem.Item, () => sellItem.Price);

                // 购买
                ShopBuyTable buyTable = new ShopBuyTable();
                foreach (var buyItem in shopConfig.BuyItems)
                {
                    if (buyItem.Countable)
                    {
                        buyTable.Add(buyItem.Item, () => buyItem.Price, () => buyItem.Count);
                    }
                    else
                    {
                        buyTable.Add(buyItem.Item, () => buyItem.Price, null);
                    }
                }

                UIShop.ShowWithPriceTables(buyTable, sellTable);
            });

            BtnShop2.onClick.AddListener(() =>
            {
                ShopConfig shopConfig = BtnShop2.GetComponent<ShopConfig>();
                UIShop.Title.text = shopConfig.ShopName;

                UIShop.ShowWithPriceTables(
                    new ShopBuyTable()
                    {
                        Table = new Dictionary<IItem, ShopBuyTable.BuyItem>()
                        {
                            {
                                Items.item_iron,
                                new ShopBuyTable.BuyItem()
                                {
                                    Item = Items.item_iron,
                                    PriceGetter = () => ItemKit.GetSlotGroupByKey("物品栏")
                                        .GetItemCount(Items.item_green_sword) + 5,
                                    CountGetter = () => mIronCount,
                                    OnBuy = () => mIronCount--,
                                }
                            }
                        },
                    },

                    new ShopSellTable()
                    {
                        Table = new Dictionary<IItem, ShopSellTable.SellItem>()
                            {
                                {
                                    Items.item_iron,
                                    new ShopSellTable.SellItem()
                                    {
                                        Item = Items.item_iron,
                                        PriceGetter = () => 5,
                                        OnSell = () => mIronCount++,
                                    }
                                },
                                {
                                    Items.item_green_sword,
                                    new ShopSellTable.SellItem()
                                    {
                                        Item = Items.item_green_sword,
                                        PriceGetter = () => ItemKit.GetSlotGroupByKey("物品栏")
                                        .GetItemCount(Items.item_green_sword) * 2,
                                        OnSell = () => { },
                                    }
                                },
                            }
                    });
            });

            // 语言
            void UpdateLanguageText()
            {
                if (ItemKit.CurrentLanguage == ItemKit.DefaultLanguage)
                    BtnLanguage.GetComponentInChildren<Text>().text = "简->EN";
                else
                    BtnLanguage.GetComponentInChildren<Text>().text = "EN->简";
            }

            UpdateLanguageText();

            BtnLanguage.onClick.AddListener(() =>
            {
                if (ItemKit.CurrentLanguage == ItemKit.DefaultLanguage)
                    ItemKit.LoadItemLanguagePackage("ItemEnglishPackage");
                else
                    ItemKit.LoadItemLanguagePackage(ItemKit.DefaultLanguage);

                UpdateLanguageText();
            });

            // 等一帧后，自动选择一个物品栏
            ActionKit.NextFrame(() =>
            {
                GetComponent<UISlotGroup>().UISlotRoot.GetChild(0).GetComponent<Selectable>().Select();

            }).Start(this);
        }

        private void OnApplicationQuit()
        {
            ItemKit.Save();
        }
    }
}
