using UnityEngine;
using QFramework;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace QFramework.Example
{
    public partial class InventoryExample1 : ViewController
    {
        private void Awake()
        {
            ItemKit.LoadItemDatabase("ExampleItemDatabase");

            ItemKit.CreateSlotGroup("物品栏")
                .CreateSlot(ItemKit.ItemByKey[Items.item_iron], 1)
                .CreateSlot(ItemKit.ItemByKey[Items.item_green_sword], 1);
        }

        private void OnGUI()
        {
            // 先规定一下分辨率
            IMGUIHelper.SetDesignResolution(960, 540);

            foreach (var slot in ItemKit.GetSlotGroupByKey("物品栏").Slots)
            {
                GUILayout.BeginHorizontal("box");
                if (slot.Count <= 0)
                    GUILayout.Label($"格子：空");
                else
                    GUILayout.Label($"格子：{slot.Item.GetName} x {slot.Count}");
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label(ItemKit.ItemByKey[Items.item_iron].GetName);
            if (GUILayout.Button("+")) ItemKit.GetSlotGroupByKey("物品栏").AddItem(Items.item_iron);
            if (GUILayout.Button("-")) ItemKit.GetSlotGroupByKey("物品栏").RemoveItem(Items.item_iron);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(ItemKit.ItemByKey[Items.item_green_sword].GetName);
            if (GUILayout.Button("+")) ItemKit.GetSlotGroupByKey("物品栏").AddItem(Items.item_green_sword);
            if (GUILayout.Button("-")) ItemKit.GetSlotGroupByKey("物品栏").RemoveItem(Items.item_green_sword);
            GUILayout.EndHorizontal();
        }

    }
}
