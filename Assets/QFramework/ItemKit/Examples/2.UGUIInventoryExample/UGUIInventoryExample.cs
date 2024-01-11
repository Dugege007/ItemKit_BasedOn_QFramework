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
            ItemKit.AddItemConfig(ConfigManager.Iron.Value);
            ItemKit.AddItemConfig(ConfigManager.GreenSword.Value);

            ItemKit.CreateSlot(ConfigManager.Iron.Value, 1);
            ItemKit.CreateSlot(ConfigManager.GreenSword.Value, 1);
        }

        private void Start()
        {
            UISlot.Hide();
            Refresh();

            #region 添加物品
            BtnAddItem1.onClick.AddListener(() =>
            {
                ItemKit.AddItem(ConfigManager.Iron.Value.GetKey);
                Refresh();
            });

            BtnAddItem2.onClick.AddListener(() =>
            {
                ItemKit.AddItem(ConfigManager.GreenSword.Value.GetKey);
                Refresh();
            });
            #endregion

            #region 删除物品
            BtnRemoveItem1.onClick.AddListener(() =>
            {
                ItemKit.RemoveItem(ConfigManager.Iron.Value.GetKey);
                Refresh();
            });

            BtnRemoveItem2.onClick.AddListener(() =>
            {
                ItemKit.RemoveItem(ConfigManager.GreenSword.Value.GetKey);
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
