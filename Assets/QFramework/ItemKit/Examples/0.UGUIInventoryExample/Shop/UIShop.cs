using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace QFramework.Example
{
    [Serializable]
    public class ShopBuyItem
    {
        public ItemConfig Item;
        public int Count;
        public int Price;
        public Func<int> PriceGetter;
    }

    [Serializable]
    public class ShopSellItem
    {
        public ItemConfig Item;
        public int Count;
        public int Price;
        public Func<int> PriceGetter;
    }

    public partial class UIShop : ViewController
    {
        public static BindableProperty<int> Coin = new(100);

        public enum ShopMode
        {
            Buy,
            Sell,
        }

        public ShopMode Mode = ShopMode.Buy;

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

            BtnBuy.onClick.AddListener(() =>
            {
                Show(mBuyItems);
            });

            BtnSell.onClick.AddListener(() =>
            {
                RefreshSellItems();
            });

            ItemKit.GetSlotGroupByKey("物品栏").Changed.Register(() =>
            {
                if (Mode == ShopMode.Sell)
                    RefreshSellItems();

            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnGUI()
        {
            IMGUIHelper.SetDesignResolution(640, 360);
            GUILayout.Label("Coin: " + Coin.Value);
        }

        private IEnumerable<ShopBuyItem> mBuyItems;

        public void Show(IEnumerable<ShopBuyItem> buyItems)
        {
            Mode = ShopMode.Buy;
            mBuyItems = buyItems;

            ShopItemRoot.DestroyChildren();

            foreach (var buyItem in buyItems)
            {
                IItem item = buyItem.Item;

                UIShopItem uiShopItem = UIShopItem
                    .InstantiateWithParent(ShopItemRoot)
                    .InitWithData(item, buyItem.Count)
                    .Show();

                ShopBuyItem uiShopItemTemp = buyItem;

                Coin.RegisterWithInitValue(coin =>
                {
                    if (buyItem.PriceGetter != null)
                        buyItem.Price = buyItem.PriceGetter();

                    uiShopItem.UpdateBuyPriceText(Coin.Value, uiShopItemTemp.Price);

                }).UnRegisterWhenGameObjectDestroyed(uiShopItem.gameObject);

                uiShopItem.BtnBuyOrSell.onClick.AddListener(() =>
                {
                    ItemKit.GetSlotGroupByKey("物品栏").AddItem(item);
                    Coin.Value -= buyItem.Price;

                    if (buyItem.PriceGetter != null)
                    {
                        buyItem.Price = buyItem.PriceGetter();
                        uiShopItem.UpdateBuyPriceText(Coin.Value, uiShopItemTemp.Price);
                    }
                });
            }

            this.Show();
        }

        public void Show(IEnumerable<ShopSellItem> sellItems)
        {
            Mode = ShopMode.Sell;

            ShopItemRoot.DestroyChildren();

            foreach (var sellItem in sellItems)
            {
                int price = sellItem.Price;
                IItem item = sellItem.Item;

                UIShopItem uiShopItem = UIShopItem.InstantiateWithParent(ShopItemRoot);
                uiShopItem.UISlot.InitWithData(new Slot(item, sellItem.Count, ItemKit.GetSlotGroupByKey("商店")));
                uiShopItem.UISlot.Count.Hide();
                uiShopItem.Name.text = item.GetName;
                uiShopItem.Description.text = item.GetDescription;
                uiShopItem.PriceText.text = price.ToString();

                ShopSellItem shopSellItem = sellItem;
                uiShopItem.BtnBuyOrSell.onClick.AddListener(() =>
                {
                    Coin.Value += price;
                    ItemKit.GetSlotGroupByKey("物品栏").RemoveItem(item);
                    RefreshSellItems();
                });

                Coin.RegisterWithInitValue(coin =>
                {
                    uiShopItem.BtnBuyOrSell.interactable = true;
                    uiShopItem.PriceText.color = Color.green;

                }).UnRegisterWhenGameObjectDestroyed(uiShopItem.gameObject);

                uiShopItem.Show();
            }

            this.Show();
        }

        private void RefreshSellItems()
        {
            HashSet<ShopSellItem> sellItems = new HashSet<ShopSellItem>();

            foreach (var slotItem in ItemKit.GetSlotGroupByKey("物品栏").Slots
                .Where(s => s.Count > 0)
                .Select(s => s.Item)
                .ToHashSet())
            {
                sellItems.Add(new ShopSellItem() { Item = slotItem as ItemConfig, Count = 1, Price = 5 });
            }
        }
    }
}
