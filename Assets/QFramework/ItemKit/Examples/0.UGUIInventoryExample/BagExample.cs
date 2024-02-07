using UnityEngine;
using QFramework;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace QFramework.Example
{
	public partial class BagExample : ViewController
	{
		private void Start()
		{
			UISlot.Hide();

            ItemKit.CreateSlotGroup("背包")
                .CreateSlotsByCount(20);

            Refresh();
        }

		public void Refresh()
		{
            SlotItemRoot.DestroyChildren();

            foreach (var slot in ItemKit.GetSlotGroupByKey("背包").Slots)
            {
                UISlot.InstantiateWithParent(SlotItemRoot)
                    .InitWithData(slot)
                    .Show();
            }
        }
    }
}
