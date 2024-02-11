using UnityEngine;
using QFramework;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace QFramework.Example
{
    public partial class UIShopItem : ViewController
    {
        public void UpdateBuyPriceText(int currentCoin, int price)
        {
            PriceText.text = price.ToString();

            if (currentCoin >= price)
            {
                BtnBuyOrSell.interactable = true;
                PriceText.color = Color.white;
            }
            else
            {
                BtnBuyOrSell.interactable = false;
                PriceText.color = Color.red;
            }
        }

        public UIShopItem InitWithData(IItem item, int count = 1)
        {
            UISlot.InitWithData(new Slot(item, count, ItemKit.GetSlotGroupByKey("商店")));
            Name.text = item.GetName;
            Description.text = item.GetDescription;

            return this;
        }
    }
}
