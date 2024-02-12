using System;
using System.Collections.Generic;

namespace QFramework.Example
{
    public class ShopBuyTable
    {
        public class BuyItem
        {
            public IItem Item;
            public Func<int> PriceGetter;
            public bool Countable;
            public int Count;
        }

        public Dictionary<IItem, BuyItem> Table = new Dictionary<IItem, BuyItem>();

        public void AddCountable(IItem item, Func<int> priceGetter, int count)
        {
            Table.Add(item, new BuyItem()
            {
                Item = item,
                PriceGetter = priceGetter,
                Countable = true,
                Count = count
            });
        }

        public void Add(IItem item, Func<int> priceGetter)
        {
            Table.Add(item, new BuyItem()
            {
                Item = item,
                PriceGetter = priceGetter,
                Countable = false,
                Count = 0
            });
        }

        public int GetPrice(IItem item)
        {
            return Table[item].PriceGetter();
        }

        public int GetCount(IItem item)
        {
            return Table[item].Count;
        }
    }
}
