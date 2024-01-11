using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using System.Text;

namespace QFramework
{
    public class ItemKitMenu
    {
        [MenuItem("QFramework/ItemKit/Create Code")]
        public static void CreateCode()
        {
            // 使用 AssetDatabase 查找所有的 ItemConfig 资源
            ItemConfig[] itemConfigs = AssetDatabase.FindAssets($"t:{nameof(ItemConfig)}")
                // 将找到的 GUID 转换为资源的文件路径
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                // 加载每个文件路径对应的 ItemConfig 对象
                .Select(assetPath => AssetDatabase.LoadAssetAtPath<ItemConfig>(assetPath))
                // 将结果转换为数组
                .ToArray();

            string filePath = "Assets/QFramework/ItemKit/Examples/Items.cs";

            // 使用 QFramework 中的代码生成功能
            // 创建一个代码作用域树，用于生成代码结构
            ICodeScope rootCode = new RootCode()
                // 添加命名空间
                .Using("UnityEngine")
                .Using("QFramework")
                // 空一行
                .EmptyLine()
                // 定义命名空间
                .Namespace("QFramework.Example", ns =>
                {
                    // 在命名空间中定义一个类
                    ns.Class("Items", String.Empty, false, false, c =>
                    {
                        // 为每个 ItemConfig 生成一个静态字符串字段
                        foreach (ItemConfig itemConfig in itemConfigs)
                        {
                            c.Custom($"public static string {itemConfig.name} = \"{itemConfig.Key}\";");
                            Debug.Log(itemConfig.Key);
                        }
                    });
                });

            // 创建一个 StringBuilder 用于构建生成的代码文本
            StringBuilder stringBuilder = new StringBuilder();
            // 创建一个代码写入器，将代码作用域树转换为字符串
            StringCodeWriter codeWriter = new StringCodeWriter(stringBuilder);
            // 生成代码
            rootCode.Gen(codeWriter);
            // 将生成的代码转换为字符串并打印到控制台
            stringBuilder.ToString().LogInfo();
        }
    }
}
