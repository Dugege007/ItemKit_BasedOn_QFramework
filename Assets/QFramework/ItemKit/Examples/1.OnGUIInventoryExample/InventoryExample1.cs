using UnityEngine;
using QFramework;
using System.Collections.Generic;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace QFramework.Example
{
    public partial class InventoryExample1 : ViewController
    {
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
                    GUILayout.Label($"格子：{slot.Item.Name} x {slot.Count}");
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label(ItemKit.Item1.Name);
            if (GUILayout.Button("+")) ItemKit.AddItem(ItemKit.Item1.Key);
            if (GUILayout.Button("-")) ItemKit.RemoveItem(ItemKit.Item1.Key);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(ItemKit.Item2.Name);
            if (GUILayout.Button("+")) ItemKit.AddItem(ItemKit.Item2.Key);
            if (GUILayout.Button("-")) ItemKit.RemoveItem(ItemKit.Item2.Key);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(ItemKit.Item3.Name);
            if (GUILayout.Button("+")) ItemKit.AddItem(ItemKit.Item3.Key);
            if (GUILayout.Button("-")) ItemKit.RemoveItem(ItemKit.Item3.Key);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(ItemKit.Item4.Name);
            if (GUILayout.Button("+")) ItemKit.AddItem(ItemKit.Item4.Key);
            if (GUILayout.Button("-")) ItemKit.RemoveItem(ItemKit.Item4.Key);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(ItemKit.Item5.Name);
            if (GUILayout.Button("+")) ItemKit.AddItem(ItemKit.Item5.Key);
            if (GUILayout.Button("-")) ItemKit.RemoveItem(ItemKit.Item5.Key);
            GUILayout.EndHorizontal();
        }

    }
}
