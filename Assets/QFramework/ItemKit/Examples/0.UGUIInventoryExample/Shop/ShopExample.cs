using UnityEngine;
using QFramework;
using UnityEngine.UI;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace QFramework.Example
{
    public partial class ShopExample : ViewController
    {
        private void Start()
        {
            UIShopItem.Hide();

            UIShopItem shopItem = UIShopItem.InstantiateWithParent(ShopItemRoot);
            shopItem.UISlot.InitWithData(new Slot(Items.item_iron, 1, ItemKit.GetSlotGroupByKey("商店")));
            shopItem.Name.text = Items.item_iron.GetName;
            shopItem.Description.text = Items.item_iron.GetDescription;
            shopItem.BtnBuy.onClick.AddListener(() =>
            {
                ItemKit.GetSlotGroupByKey("物品栏").AddItem(Items.item_iron);
            });

            shopItem.Show();
        }
    }
}
