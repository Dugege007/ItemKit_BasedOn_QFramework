using UnityEngine;
using QFramework;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace QFramework.Example
{
    public partial class UICraft : ViewController
    {
        private void Start()
        {
            CraftItem.Hide();

            // 假设需要 3 个铁块，生成 1 个宝石
            int needCount = 3;
            int gCount = 1;
            CraftItem craftItem = CraftItem.InstantiateWithParent(CraftItemRoot);

            craftItem.CostItemSlot.Hide();

            // 需要
            craftItem.CostItemSlot.InstantiateWithParent(craftItem.CostItemRoot)
                .Self(self =>
                {
                    self.InitWithData(new Slot(Items.item_iron, needCount, ItemKit.GetSlotGroupByKey("锻造")));
                })
                .Show();

            // 生成
            craftItem.NeedItemSlot.InitWithData(new Slot(Items.item_green_sword, gCount, ItemKit.GetSlotGroupByKey("锻造")));

            SlotGroup slotGroup = ItemKit.GetSlotGroupByKey("物品栏");

            void UpdateButtonView()
            {
                int count = slotGroup.GetItemCount(Items.item_iron);
                if (count >= needCount)
                    craftItem.BtnCraft.interactable = true;
                else
                    craftItem.BtnCraft.interactable = false;
            }

            UpdateButtonView();

            slotGroup.Changed.Register(() =>
            {
                UpdateButtonView();

            }).UnRegisterWhenGameObjectDestroyed(craftItem.gameObject);

            craftItem.BtnCraft.onClick.AddListener(() =>
            {
                int count = slotGroup.GetItemCount(Items.item_iron);
                if (count >= needCount)
                {
                    slotGroup.RemoveItem(Items.item_iron, needCount);
                    slotGroup.AddItem(Items.item_green_sword);
                }
            });

            craftItem.Show();
        }
    }
}
