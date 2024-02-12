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
        public int Price;

        [LabelText("”–ø‚¥Ê")]
        public bool Countable = false;
        [ShowIf("Countable", true)]
        public int Count;
    }

    [Serializable]
    public class ShopSellItem
    {
        public ItemConfig Item;
        public int Price;
        public int Count;
    }

    public class ShopConfig : MonoBehaviour
    {
        [LabelText("…ÃµÍ√˚")]
        public string ShopName;

        public List<ShopBuyItem> BuyItems;
        public List<ShopSellItem> SellItems;
    }
}