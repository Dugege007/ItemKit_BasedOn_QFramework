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
			UISlot.Hide();

            foreach (var slot in ItemKit.Slots)
            {
				UISlot.InstantiateWithParent(UISlotRoot)
					.Self(self => // 拿到 Instantiate 之后的这个 UISlot
                    {
						if (slot.Count == 0)
						{
							self.Name.text = "";
							self.Count.text = "";
						}
						else
						{
                            self.Name.text = slot.Item.Name;
                            self.Count.text = slot.Count.ToString();
                        }
                    })
					.Show();
            }
        }
	}
}
