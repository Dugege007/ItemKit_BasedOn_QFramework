using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;  // 对 using UnityEditor 也加上宏，避免打包出现问题
#endif

namespace QFramework
{
    [CreateAssetMenu(menuName = "@ItemKit/Create Item Database")]
    public class ItemDatabase : ScriptableObject
    {
        public string NameSpace = "QFramework.Example";

        public List<ItemConfig> ItemConfigs = new List<ItemConfig>();

#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(ItemDatabase))]
        public class ItemDatabaseEditor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                if (GUILayout.Button("Create Code"))
                {
                    //// 使用 AssetDatabase 查找所有的 ItemConfig 资源
                    //ItemConfig[] itemConfigs = AssetDatabase.FindAssets($"t:{nameof(ItemConfig)}")
                    //    // 将找到的 GUID 转换为资源的文件路径
                    //    .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                    //    // 加载每个文件路径对应的 ItemConfig 对象
                    //    .Select(assetPath => AssetDatabase.LoadAssetAtPath<ItemConfig>(assetPath))
                    //    // 将结果转换为数组
                    //    .ToArray();

                    // 此时 itemConfigs 不用去搜索了，可以直接获取
                    // target 指的是次编辑器对应的脚本，也就是 ItemDatabase
                    var itemDB = target as ItemDatabase;

                    // 文件保存路径与此脚本相同
                    string filePath = AssetDatabase.GetAssetPath(target).GetFolderPath() + "/Items.cs";

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
                                // 为每个 itemDB.ItemConfigs 生成一个静态字符串字段
                                foreach (ItemConfig itemConfig in itemDB.ItemConfigs)
                                {
                                    c.Custom($"public static string {itemConfig.name} = \"{itemConfig.Key}\";");
                                    Debug.Log(itemConfig.Key);
                                }
                            });
                        });

                    // 创建或覆盖文件，并准备写入生成的代码
                    // 使用 using 语句自动管理 StreamWriter 的生命周期。
                    // 当离开 using 代码块的作用域时，fileWriter 的 Dispose 方法会被自动调用，确保文件资源被正确关闭。
                    using StreamWriter fileWriter = File.CreateText(filePath);
                    // 创建一个代码写入器，将代码作用域树转换为字符串
                    FileCodeWriter codeWriter = new FileCodeWriter(fileWriter);
                    // 生成代码并写入文件
                    rootCode.Gen(codeWriter);

                    // 保存所有未保存的资源更改
                    AssetDatabase.SaveAssets();
                    // 刷新 Unity 编辑器的资源数据库
                    AssetDatabase.Refresh();
                }
            }
        }
#endif
    }
}
