using System;
using System.Collections.Generic;

namespace QFramework.Example
{
    public class ShopSellTable
    {
        public class SellItem
        {
            public IItem Item;
            public Func<int> PriceGetter;
            public Action OnSell;
        }

        public Dictionary<IItem, SellItem> Table = new Dictionary<IItem, SellItem>();

        public void Add(IItem item, Func<int> priceGetter, Action onSell = null)
        {
            Table.Add(item, new SellItem()
            {
                Item = item,
                PriceGetter = priceGetter,
                OnSell = onSell
            });
        }

        public int GetPrice(IItem item)
        {
            return Table[item].PriceGetter();
        }
    }
}
