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

            ItemKit.CreateSlot(ItemKit.ItemByKey[Items.item_iron], 1);
            ItemKit.CreateSlot(ItemKit.ItemByKey[Items.item_green_sword], 1);
        }

        private void Start()
        {
            UISlot.Hide();
            Refresh();

            #region 添加物品
            BtnAddItem1.onClick.AddListener(() =>
            {
                ItemKit.AddItem(Items.item_iron);
                Refresh();
            });

            BtnAddItem2.onClick.AddListener(() =>
            {
                ItemKit.AddItem(Items.item_green_sword);
                Refresh();
            });
            #endregion

            #region 删除物品
            BtnRemoveItem1.onClick.AddListener(() =>
            {
                ItemKit.RemoveItem(Items.item_iron);
                Refresh();
            });

            BtnRemoveItem2.onClick.AddListener(() =>
            {
                ItemKit.RemoveItem(Items.item_green_sword);
                Refresh();
            });
            #endregion
        }

        public void Refresh()
        {
            UISlotRoot.DestroyChildren();   // 暂时不考虑性能优化

            foreach (var slot in ItemKit.Slots)
            {
                UISlot.InstantiateWithParent(UISlotRoot)
                    .InitWithData(slot)
                    .Show();
            }
        }
    }
}
