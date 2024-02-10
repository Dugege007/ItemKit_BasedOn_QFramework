using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace QFramework.Example
{
    public partial class ShopExample : ViewController
    {
        public static BindableProperty<int> Coin = new(100);

        private ShopComponent mShopComponent;

        private void Awake()
        {
            mShopComponent = GetComponent<ShopComponent>();

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
            List<ShopComponent.ShopBuyItem> buyItems = new List<ShopComponent.ShopBuyItem>()
            {
                new ShopComponent.ShopBuyItem() { Item = Items.item_green_sword as ItemConfig, Count = 1, Price = 150 },
                new ShopComponent.ShopBuyItem() { Item = Items.item_iron as ItemConfig, Count = 1, Price = 20 },
                new ShopComponent.ShopBuyItem() { Item = Items.item_iron as ItemConfig, Count = 5, Price = 90 },
                new ShopComponent.ShopBuyItem() { Item = Items.item_wood as ItemConfig, Count = 1, Price = 10 },
                new ShopComponent.ShopBuyItem() { Item = Items.item_paper as ItemConfig, Count = 1, Price = 2 },
            };

            mShopComponent.Show(buyItems, (uiShopItem, item) =>
            {
                Coin.RegisterWithInitValue(coin =>
                {
                    if (coin >= item.Price)
                    {
                        uiShopItem.BtnBuyOrSell.interactable = true;
                        uiShopItem.PriceText.color = Color.white;
                    }
                    else
                    {
                        uiShopItem.BtnBuyOrSell.interactable = false;
                        uiShopItem.PriceText.color = Color.red;
                    }

                }).UnRegisterWhenGameObjectDestroyed(uiShopItem.gameObject);

                uiShopItem.BtnBuyOrSell.onClick.AddListener(() =>
                {
                    ItemKit.GetSlotGroupByKey("物品栏").AddItem(item.Item);
                    Coin.Value -= item.Price;
                });
            });
        }

        private void RefreshSellItems()
        {
            HashSet<ShopComponent.ShopSellItem> sellItems = new HashSet<ShopComponent.ShopSellItem>();

            foreach (var slotItem in ItemKit.GetSlotGroupByKey("物品栏").Slots
                .Where(s => s.Count > 0)
                .Select(s => s.Item)
                .ToHashSet())
            {
                sellItems.Add(new ShopComponent.ShopSellItem() { Item = slotItem as ItemConfig, Count = 1, Price = 5 });
            }

            mShopComponent.Show(sellItems, item =>
            {
                Coin.Value += item.Price;
                ItemKit.GetSlotGroupByKey("物品栏").RemoveItem(item.Item);
                RefreshSellItems();
            });
        }
    }
}
