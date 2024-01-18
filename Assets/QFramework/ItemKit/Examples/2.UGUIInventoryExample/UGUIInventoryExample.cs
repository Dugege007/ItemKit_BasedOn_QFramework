using UnityEngine;
using QFramework;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace QFramework.Example
{
    public partial class UGUIInventoryExample : ViewController
    {
        private void Awake()
        {
            ItemKit.LoadItemDatabase("ExampleItemDatabase");

            ItemKit.CreateSlotGroup("物品栏")
                .CreateSlot(ItemKit.ItemByKey[Items.item_iron], 1)
                .CreateSlot(ItemKit.ItemByKey[Items.item_green_sword], 1)
                .CreateSlotsByCount(6);

            ItemKit.CreateSlotGroup("背包")
                .CreateSlotsByCount(20);

            ItemKit.CreateSlotGroup("武器")
                .CreateSlot(null, 0);
        }

        private void Start()
        {
            #region 添加物品
            BtnAddItem1.onClick.AddListener(() =>
            {
                ItemKit.GetSlotGroupByKey("物品栏") // 先拿到背包
                    .AddItem(Items.item_iron);    // 再添加物品
            });

            BtnAddItem2.onClick.AddListener(() =>
            {
                ItemKit.GetSlotGroupByKey("物品栏")
                    .AddItem(Items.item_green_sword);
            });

            BtnAddItem3.onClick.AddListener(() =>
            {
                ItemKit.GetSlotGroupByKey("物品栏")
                    .AddItem(Items.item_paper);
            });
            #endregion

            #region 删除物品
            BtnRemoveItem1.onClick.AddListener(() =>
            {
                ItemKit.GetSlotGroupByKey("物品栏")
                    .RemoveItem(Items.item_iron);
            });

            BtnRemoveItem2.onClick.AddListener(() =>
            {
                ItemKit.GetSlotGroupByKey("物品栏")
                    .RemoveItem(Items.item_green_sword);
            });

            BtnRemoveItem3.onClick.AddListener(() =>
            {
                ItemKit.GetSlotGroupByKey("物品栏")
                    .RemoveItem(Items.item_paper);
            });
            #endregion
        }
    }
}
