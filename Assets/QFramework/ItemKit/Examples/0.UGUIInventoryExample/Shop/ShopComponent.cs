using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework.Example
{
    public class ShopComponent : MonoBehaviour
    {
        public UIShopItem UIShopItemTemplate;
        public RectTransform ShopItemRoot;

        [Serializable]
        public class ShopBuyItem
        {
            public ItemConfig Item;
            public int Count;
            public int Price;
        }

        [Serializable]
        public class ShopSellItem
        {
            public ItemConfig Item;
            public int Count;
            public int Price;
        }

        private void Start()
        {
            UIShopItemTemplate.Hide();
        }

        public void Show(IEnumerable<ShopBuyItem> buyItems, Action<UIShopItem, ShopBuyItem> setup)
        {
            ShopItemRoot.DestroyChildren();

            foreach (var buyItem in buyItems)
            {
                int price = buyItem.Price;
                IItem item = buyItem.Item;

                UIShopItem shopItem = UIShopItemTemplate.InstantiateWithParent(ShopItemRoot);
                shopItem.UISlot.InitWithData(new Slot(item, buyItem.Count, ItemKit.GetSlotGroupByKey("ил╣Й")));
                shopItem.Name.text = item.GetName;
                shopItem.Description.text = item.GetDescription;
                shopItem.PriceText.text = price.ToString();

                shopItem.Show();
            }
        }

        public void Show(IEnumerable<ShopSellItem> sellItems, Action<ShopSellItem> onSellClick)
        {
            ShopItemRoot.DestroyChildren();

            foreach (var sellItem in sellItems)
            {
                int price = sellItem.Price;
                IItem item = sellItem.Item;

                UIShopItem shopItem = UIShopItemTemplate.InstantiateWithParent(ShopItemRoot);
                shopItem.UISlot.InitWithData(new Slot(item, sellItem.Count, ItemKit.GetSlotGroupByKey("ил╣Й")));
                shopItem.UISlot.Count.Hide();
                shopItem.Name.text = item.GetName;
                shopItem.Description.text = item.GetDescription;
                shopItem.PriceText.text = price.ToString();

                ShopSellItem shopSellItem = sellItem;
                shopItem.BtnBuyOrSell.onClick.AddListener(() =>
                {
                    onSellClick(shopSellItem);
                });

                ShopExample.Coin.RegisterWithInitValue(coin =>
                {
                    shopItem.BtnBuyOrSell.interactable = true;
                    shopItem.PriceText.color = Color.green;

                }).UnRegisterWhenGameObjectDestroyed(shopItem.gameObject);

                shopItem.Show();
            }
        }
    }
}