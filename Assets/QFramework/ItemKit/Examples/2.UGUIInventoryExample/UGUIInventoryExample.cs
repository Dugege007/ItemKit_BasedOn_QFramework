using UnityEngine;
using QFramework;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace QFramework.Example
{
    public partial class UGUIInventoryExample : ViewController
    {
        private void Start()
        {
            ItemKit.AddItemConfig(ConfigManager.Default.Iron);
            ItemKit.AddItemConfig(ConfigManager.Default.GreenSword);

            ItemKit.Slots[0].Item = ConfigManager.Default.Iron;
            ItemKit.Slots[0].Count = 1;

            UISlot.Hide();
            Refresh();

            #region 添加物品
            BtnAddItem1.onClick.AddListener(() =>
            {
                ItemKit.AddItem(ConfigManager.Default.Iron.Key);
                Refresh();
            });

            BtnAddItem2.onClick.AddListener(() =>
            {
                ItemKit.AddItem(ConfigManager.Default.GreenSword.Key);
                Refresh();
            });

            BtnAddItem3.onClick.AddListener(() =>
            {
                ItemKit.AddItem(ItemKit.Item3.Key);
                Refresh();
            });

            BtnAddItem4.onClick.AddListener(() =>
            {
                ItemKit.AddItem(ItemKit.Item4.Key);
                Refresh();
            });

            BtnAddItem5.onClick.AddListener(() =>
            {
                ItemKit.AddItem(ItemKit.Item5.Key);
                Refresh();
            });
            #endregion

            #region 删除物品
            BtnRemoveItem1.onClick.AddListener(() =>
            {
                ItemKit.RemoveItem(ConfigManager.Default.Iron.Key);
                Refresh();
            });

            BtnRemoveItem2.onClick.AddListener(() =>
            {
                ItemKit.RemoveItem(ConfigManager.Default.GreenSword.Key);
                Refresh();
            });

            BtnRemoveItem3.onClick.AddListener(() =>
            {
                ItemKit.RemoveItem(ItemKit.Item3.Key);
                Refresh();
            });

            BtnRemoveItem4.onClick.AddListener(() =>
            {
                ItemKit.RemoveItem(ItemKit.Item4.Key);
                Refresh();
            });

            BtnRemoveItem5.onClick.AddListener(() =>
            {
                ItemKit.RemoveItem(ItemKit.Item5.Key);
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
