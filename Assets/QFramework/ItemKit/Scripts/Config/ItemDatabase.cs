using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEditor;

namespace QFramework
{
    [CreateAssetMenu(menuName = "@ItemKit/Create Item Database")]
    public class ItemDatabase : ScriptableObject
    {
        public string NameSpace = "QFramework.Example";

        [Searchable]
        [TableList(ShowIndexLabels = true)]
        public List<ItemConfig> ItemConfigs = new List<ItemConfig>();

#if UNITY_EDITOR
        [Button("添加 ItemConfig", ButtonSizes.Large), GUIColor("yellow")]
        private void AddItemConfig()
        {
            // 创建一个新的 ItemConfig 实例
            ItemConfig itemConfigSO = CreateInstance<ItemConfig>();
            itemConfigSO.ItemDatabase = this;
            itemConfigSO.name = nameof(ItemConfig);
            itemConfigSO.Name = "新物品";
            itemConfigSO.Key = "item_new";

            // 将新创建的 itemConfig 添加到 ItemConfigGroup 的资源中
            AssetDatabase.AddObjectToAsset(itemConfigSO, this);
            // 在 ItemConfigs 列表中添加一个新的元素
            ItemConfigs.Add(itemConfigSO);

            Debug.Log($"添加物品配置: {itemConfigSO.GetKey}");
            // 保存所有更改到资源
            AssetDatabase.SaveAssets();
            // 刷新资源
            AssetDatabase.Refresh();
        }

        public void DuplicateItemConfig(int index, ItemConfig itemConfig)
        {
            // 创建一个新的 ItemConfig 实例
            ItemConfig itemConfigSO = CreateInstance<ItemConfig>();
            itemConfigSO.ItemDatabase = this;
            itemConfigSO.name = itemConfig.Key;
            itemConfigSO.Name = string.Empty;
            itemConfigSO.Key = "item_new";
            itemConfigSO.IsWeapon = itemConfig.IsWeapon;
            itemConfigSO.IsStackable = itemConfig.IsStackable;
            itemConfigSO.HasMaxStackableCount = itemConfig.HasMaxStackableCount;
            itemConfigSO.MaxStackableCount = itemConfig.MaxStackableCount;

            // 将新创建的 itemConfig 添加到 ItemConfigGroup 的资源文件中
            AssetDatabase.AddObjectToAsset(itemConfigSO, this);
            // 在 ItemConfigs 列表中添加一个新的元素
            ItemConfigs.Insert(index + 1, itemConfigSO);

            // 保存所有更改到资源
            AssetDatabase.SaveAssets();
            // 刷新资源
            AssetDatabase.Refresh();
        }

        [Button("生成 Items 代码", ButtonSizes.Large), GUIColor("green")]
        private void GenerateCode()
        {
            var itemDatabase = this;
            // 获取当前 ItemDatabase 脚本的文件路径，并确定生成代码的保存位置
            string filePath = AssetDatabase.GetAssetPath(itemDatabase).GetFolderPath() + "/Items.cs";

            // 使用 QFramework 中的代码生成功能
            // 创建一个代码作用域树，用于生成代码结构
            ICodeScope rootCode = new RootCode()
                // 添加命名空间
                .Using("UnityEngine")
                .Using("QFramework")
                // 空一行
                .EmptyLine()
                // 定义命名空间
                .Namespace(itemDatabase.NameSpace, ns =>
                {
                    // 在命名空间中定义一个类
                    ns.Class("Items", String.Empty, false, false, c =>
                    {
                        // 为每个 itemDB.ItemConfigs 生成一个静态字符串字段
                        foreach (ItemConfig itemConfig in itemDatabase.ItemConfigs)
                        {
                            // Items 类试图在其静态字段初始化时直接从 ItemKit.ItemByKey 字典中访问项目
                            // 但这些项目可能还没有被添加到字典中
                            // 为了更便捷地解决这一问题，可以时候用延迟加载
                            c.Custom($"private static IItem _{itemConfig.Key};");
                            c.Custom($"public static IItem {itemConfig.Key} " +
                                $"{{ get " +
                                $"{{ if (_{itemConfig.Key} == null) " +
                                $"_{itemConfig.Key} = ItemKit.ItemByKey[\"{itemConfig.Key}\"]; " +
                                $"return _{itemConfig.Key}; " +
                                $"}}" +
                                $"}}");
                            c.Custom($"public static string {itemConfig.Key}_key = \"{itemConfig.Key}\";"); 
                            
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
#endif

        private void OnValidate()
        {
            foreach (ItemConfig itemConfig in ItemConfigs)
            {
                if (itemConfig != null)
                {
                    itemConfig.name = itemConfig.Key;
                }
                else
                    ItemConfigs.Remove(itemConfig);
            }
        }
    }
}