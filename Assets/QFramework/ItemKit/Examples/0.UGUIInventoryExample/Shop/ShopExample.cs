using UnityEngine;
using QFramework;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace QFramework.Example
{
    public partial class ShopExample : ViewController
    {
        public class ShopBuyItem
        {
            public IItem Item { get; set; }
            public int Count { get; set; }
            public int Price { get; set; }
        }

        public class ShopSellItem
        {
            public IItem Item { get; set; }
            public int Count { get; set; }
            public int Price { get; set; }
        }

        public static BindableProperty<int> Coin = new(100);

        private void Awake()
        {
            // 加载
            Coin.Value = PlayerPrefs.GetInt(nameof(Coin), 100);

            // 保存
            Coin.Register(coin =>
            {
                PlayerPrefs.SetInt(nameof(Coin), coin);

            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void Start()
        {
            UIShopItem.Hide();

            ShowBuyItems();

            BtnBuy.onClick.AddListener(() =>
            {
                ShowBuyItems();
            });

            BtnSell.onClick.AddListener(() =>
            {
                RefreshSellItems();
            });

            ItemKit.GetSlotGroupByKey("物品栏").Changed.Register(() =>
            {
                RefreshSellItems();

            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnGUI()
        {
            IMGUIHelper.SetDesignResolution(640, 360);
            GUILayout.Label("Coin: " + Coin.Value);
        }

        private void ShowBuyItems()
        {
            ShopItemRoot.DestroyChildren();

            List<ShopBuyItem> buyItems = new List<ShopBuyItem>()
            {
                new ShopBuyItem() { Item = Items.item_green_sword, Count = 1, Price = 150 },
                new ShopBuyItem() { Item = Items.item_iron, Count = 1, Price = 20 },
                new ShopBuyItem() { Item = Items.item_iron, Count = 5, Price = 90 },
                new ShopBuyItem() { Item = Items.item_wood, Count = 1, Price = 10 },
                new ShopBuyItem() { Item = Items.item_paper, Count = 1, Price = 2 },
            };

            foreach (var buyItem in buyItems)
            {
                int price = buyItem.Price;
                IItem item = buyItem.Item;

                UIShopItem shopItem = UIShopItem.InstantiateWithParent(ShopItemRoot);
                shopItem.UISlot.InitWithData(new Slot(item, buyItem.Count, ItemKit.GetSlotGroupByKey("商店")));
                shopItem.Name.text = item.GetName;
                shopItem.Description.text = item.GetDescription;
                shopItem.PriceText.text = price.ToString();
                shopItem.BtnBuyOrSell.onClick.AddListener(() =>
                {
                    ItemKit.GetSlotGroupByKey("物品栏").AddItem(item);
                    Coin.Value -= price;
                });

                Coin.RegisterWithInitValue(coin =>
                {
                    if (coin >= price)
                    {
                        shopItem.BtnBuyOrSell.interactable = true;
                        shopItem.PriceText.color = Color.white;
                    }
                    else
                    {
                        shopItem.BtnBuyOrSell.interactable = false;
                        shopItem.PriceText.color = Color.red;
                    }

                }).UnRegisterWhenGameObjectDestroyed(shopItem.gameObject);

                shopItem.Show();
            }
        }

        private void RefreshSellItems()
        {
            ShopItemRoot.DestroyChildren();

            HashSet<ShopSellItem> sellItems = new HashSet<ShopSellItem>();

            foreach (var slotItem in ItemKit.GetSlotGroupByKey("物品栏").Slots
                .Where(s => s.Count > 0)
                .Select(s => s.Item)
                .ToHashSet())
            {
                sellItems.Add(new ShopSellItem() { Item = slotItem, Count = 1, Price = 5 });
            }

            foreach (var sellItem in sellItems)
            {
                int price = sellItem.Price;
                IItem item = sellItem.Item;

                UIShopItem shopItem = UIShopItem.InstantiateWithParent(ShopItemRoot);
                shopItem.UISlot.InitWithData(new Slot(item, sellItem.Count, ItemKit.GetSlotGroupByKey("商店")));
                shopItem.UISlot.Count.Hide();
                shopItem.Name.text = item.GetName;
                shopItem.Description.text = item.GetDescription;
                shopItem.PriceText.text = price.ToString();
                shopItem.BtnBuyOrSell.onClick.AddListener(() =>
                {
                    ItemKit.GetSlotGroupByKey("物品栏").RemoveItem(item);
                    Coin.Value += price;
                    RefreshSellItems();
                });

                Coin.RegisterWithInitValue(coin =>
                {
                    shopItem.BtnBuyOrSell.interactable = true;
                    shopItem.PriceText.color = Color.green;

                }).UnRegisterWhenGameObjectDestroyed(shopItem.gameObject);

                shopItem.Show();
            }
        }
    }
}
