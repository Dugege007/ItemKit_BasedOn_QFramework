using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEditor;
using System.Linq;

namespace QFramework
{
    public enum AttributeType
    {
        Boolean,
        Int,
        Float,
        String,
    }

    [Serializable]
    public class ItemAttributeDefine
    {
        [HideLabel]
        [HorizontalGroup("名称类型和值")]
        [VerticalGroup("名称类型和值/name")]
        [LabelText("名称"), LabelWidth(42)]
        public string Name;

        [HideLabel]
        [HorizontalGroup("名称类型和值")]
        [VerticalGroup("名称类型和值/type")]
        public AttributeType Type;
    }

    [CreateAssetMenu(menuName = "@ItemKit/Create Item Database")]
    public class ItemDatabase : ScriptableObject
    {
        [LabelText("命名空间")]
        public string NameSpace = "QFramework.Example";
        [LabelText("生成路径")]
        public string CodeGenPath = "";

#if UNITY_EDITOR
        [Button("生成 Items 代码", ButtonSizes.Large), GUIColor("green")]
        private void GenerateCode()
        {
            var itemDatabase = this;
            string path = CodeGenPath;

            if (CodeGenPath.IsNullOrEmpty())
            {
                CodeGenPath = EditorUtility.SaveFilePanelInProject("Items.cs", "Items", "cs", null);
            }

            // 获取当前 ItemDatabase 脚本的文件路径，并确定生成代码的保存位置
            string filePath = path;

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
                            c.Custom($"public static IItem {itemConfig.Key} => ItemKit.ItemByKey[\"{itemConfig.Key}\"];");
                            c.Custom($"public static string {itemConfig.Key}_key = \"{itemConfig.Key}\";\n");

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

        [LabelText("属性定义"), VerticalGroup("排列", Order = 10, AnimateVisibility = true)]
        public List<ItemAttributeDefine> AttributesDefine = new List<ItemAttributeDefine>();

        [Searchable, VerticalGroup("排列", AnimateVisibility = true)]
        [TableList(ShowIndexLabels = true)]
        public List<ItemConfig> ItemConfigs = new List<ItemConfig>();

#if UNITY_EDITOR
        [Button("添加 ItemConfig", ButtonSizes.Large), GUIColor("yellow"), VerticalGroup("排列")]
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
            itemConfigSO.Attributes = new List<ItemAttribute>();
            foreach (ItemAttribute attribute in itemConfig.Attributes)
            {
                itemConfigSO.Attributes.Add(new ItemAttribute()
                {
                    Name = attribute.Name,
                    Type = attribute.Type,
                    Value = attribute.Value,
                    BoolValue = attribute.BoolValue,
                });
            }
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
#endif


        private void OnValidate()
        {
            foreach (ItemConfig itemConfig in ItemConfigs)
            {
                if (itemConfig != null)
                    itemConfig.name = itemConfig.Key;
                else
                    ItemConfigs.Remove(itemConfig);
            }

            if (AttributesDefine.Count > 0)
            {
                foreach (ItemAttributeDefine attributeDefine in AttributesDefine)
                {
                    foreach (ItemConfig item in ItemConfigs)
                    {
                        ItemAttribute attribute = item.Attributes.FirstOrDefault(attribute => attribute.Name == attributeDefine.Name);

                        // 同步
                        if (attribute == null)
                        {
                            attribute = new ItemAttribute()
                            {
                                Name = attributeDefine.Name,
                                Type = attributeDefine.Type,
                            };

                            item.Attributes.Add(attribute);
                        }
                        else
                        {
                            attribute.Type = attributeDefine.Type;
                        }

                        // 去除冗余
                        if (item.Attributes.Count > AttributesDefine.Count)
                        {
                            item.Attributes.RemoveAll(attribute => AttributesDefine.All(g => g.Name != attribute.Name));
                        }
                    }
                }
            }
            else
            {
                foreach (var item in ItemConfigs)
                {
                    item.Attributes.Clear();
                }
            }
        }
    }

    public class ItemAttributeDefineDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }


    }
}