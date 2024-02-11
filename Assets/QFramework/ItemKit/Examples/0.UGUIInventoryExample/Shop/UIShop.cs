using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace QFramework.Example
{
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
                ShowBuyItems(mBuyPriceTable);
            });

            BtnSell.onClick.AddListener(() =>
            {
                ShowSellItems(mSellPriceTable);
            });

            ItemKit.GetSlotGroupByKey("物品栏").Changed.Register(() =>
            {
                if (Mode == ShopMode.Sell)
                    ShowSellItems(mSellPriceTable);

            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnGUI()
        {
            IMGUIHelper.SetDesignResolution(640, 360);
            GUILayout.Label("Coin: " + Coin.Value);
        }

        public void ShowWithPriceTables(Dictionary<IItem, Func<int>> buyPriceTable, Dictionary<IItem, Func<int>> sellPriceTable)
        {
            mBuyPriceTable = buyPriceTable;
            mSellPriceTable = sellPriceTable;

            ShowBuyItems(mBuyPriceTable);
        }

        private Dictionary<IItem, Func<int>> mBuyPriceTable;

        public void ShowBuyItems(Dictionary<IItem, Func<int>> buyPriceTable)
        {
            Mode = ShopMode.Buy;
            mBuyPriceTable = buyPriceTable;

            ShopItemRoot.DestroyChildren();

            foreach (var buyItem in buyPriceTable)
            {
                IItem item = buyItem.Key;
                Func<int> priceGetter = buyItem.Value;

                UIShopItem uiShopItem = UIShopItem
                    .InstantiateWithParent(ShopItemRoot)
                    .InitWithData(item)
                    .Show();

                IItem uiShopItemTemp = item;

                Coin.RegisterWithInitValue(coin =>
                {
                    uiShopItem.UpdateBuyPriceText(Coin.Value, priceGetter());

                }).UnRegisterWhenGameObjectDestroyed(uiShopItem.gameObject);

                uiShopItem.BtnBuyOrSell.onClick.AddListener(() =>
                {
                    ItemKit.GetSlotGroupByKey("物品栏").AddItem(uiShopItemTemp);
                    Coin.Value -= mBuyPriceTable[uiShopItemTemp]();

                    uiShopItem.UpdateBuyPriceText(Coin.Value, priceGetter());
                });
            }

            this.Show();
        }

        private Dictionary<IItem, Func<int>> mSellPriceTable;

        public void ShowSellItems(Dictionary<IItem, Func<int>> sellPriceTable)
        {
            mSellPriceTable = sellPriceTable;
            HashSet<ShopSellItem> sellItems = new HashSet<ShopSellItem>();

            foreach (var slotItem in ItemKit.GetSlotGroupByKey("物品栏").Slots
                .Where(s => s.Count > 0)
                .Select(s => s.Item)
                .ToHashSet())
            {
                if (mSellPriceTable != null && mSellPriceTable.ContainsKey(slotItem))
                {
                    sellItems.Add(new ShopSellItem()
                    {
                        Item = slotItem as ItemConfig,
                        Count = 1,
                        Price = mSellPriceTable[slotItem]()
                    });
                }
            }

            Mode = ShopMode.Sell;

            ShopItemRoot.DestroyChildren();

            foreach (var shopSellItem in sellItems)
            {
                int price = shopSellItem.Price;
                IItem item = shopSellItem.Item;

                UIShopItem uiShopItem = UIShopItem.InstantiateWithParent(ShopItemRoot);
                uiShopItem.UISlot.InitWithData(new Slot(item, shopSellItem.Count, ItemKit.GetSlotGroupByKey("商店")));
                uiShopItem.UISlot.Count.Hide();
                uiShopItem.Name.text = item.GetName;
                uiShopItem.Description.text = item.GetDescription;
                uiShopItem.PriceText.text = price.ToString();

                uiShopItem.BtnBuyOrSell.onClick.AddListener(() =>
                {
                    Coin.Value += price;
                    ItemKit.GetSlotGroupByKey("物品栏").RemoveItem(item);
                    ShowSellItems(mSellPriceTable);
                });

                Coin.RegisterWithInitValue(coin =>
                {
                    uiShopItem.BtnBuyOrSell.interactable = true;
                    uiShopItem.PriceText.color = Color.green;

                }).UnRegisterWhenGameObjectDestroyed(uiShopItem.gameObject);

                uiShopItem.Show();
            }
        }
    }
}
