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
            ItemKit.AddItemConfig(ConfigManager.Iron.Value);
            ItemKit.AddItemConfig(ConfigManager.GreenSword.Value);

            ItemKit.CreateSlot(ConfigManager.Iron.Value, 1);
            ItemKit.CreateSlot(ConfigManager.GreenSword.Value, 1);
        }

        private void OnGUI()
        {
            // 先规定一下分辨率
            IMGUIHelper.SetDesignResolution(960, 540);

            foreach (var slot in ItemKit.Slots)
            {
                GUILayout.BeginHorizontal("box");
                if (slot.Count <= 0)
                    GUILayout.Label($"格子：空");
                else
                    GUILayout.Label($"格子：{slot.Item.GetName} x {slot.Count}");
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label(ConfigManager.Iron.Value.GetName);
            if (GUILayout.Button("+")) ItemKit.AddItem(ConfigManager.Iron.Value.GetKey);
            if (GUILayout.Button("-")) ItemKit.RemoveItem(ConfigManager.Iron.Value.GetKey);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(ConfigManager.GreenSword.Value.GetName);
            if (GUILayout.Button("+")) ItemKit.AddItem(ConfigManager.GreenSword.Value.GetKey);
            if (GUILayout.Button("-")) ItemKit.RemoveItem(ConfigManager.GreenSword.Value.GetKey);
            GUILayout.EndHorizontal();
        }

    }
}
