using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework.Example
{
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

    public class ShopConfig : MonoBehaviour
    {
        [LabelText("…ÃµÍ√˚")]
        public string ShopName;

        public List<ShopBuyItem> BuyItems;
        public List<ShopSellItem> SellItems;
    }
}