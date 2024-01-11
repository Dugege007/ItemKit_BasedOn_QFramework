using System;
using System.Collections.Generic;
using System.IO;
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
            // mItemConfigs 是一个序列化属性，用于编辑器中显示和编辑 ItemConfigs 列表
            private SerializedProperty mItemConfigs;

            private void OnEnable()
            {
                // 获取并存储 ItemConfigs 列表的序列化属性引用
                mItemConfigs = serializedObject.FindProperty("ItemConfigs");
            }

            // OnInspectorGUI 重写自定义编辑器的GUI布局和行为
            public override void OnInspectorGUI()
            {
                //base.OnInspectorGUI();

                // serializedObject 是 ItemDatabase 的可序列化的对象
                // .Update() 更新序列化对象的状态，以准备显示
                serializedObject.Update();

                // 绘制除 ItemConfigs 之外的所有属性
                // .DrawProperties() 是 QFramework 提供的，其中包含 base.OnInspectorGUI() 的功能
                // .DrawProperties(是否绘制脚本，缩进值，忽略)
                serializedObject.DrawProperties(true, 0, "ItemConfigs");

                if (GUILayout.Button("+"))
                {
                    // 创建一个新的 ItemConfig 实例
                    ItemConfig itemConfig = ItemConfig.CreateInstance<ItemConfig>();
                    // 将新创建的 itemConfig 添加到 ItemDatabase 的资产中
                    AssetDatabase.AddObjectToAsset(itemConfig, target);

                    // 在 ItemConfigs 列表中添加一个新的元素
                    mItemConfigs.InsertArrayElementAtIndex(mItemConfigs.arraySize);
                    // 获取新添加的元素的序列化属性，并设置其值为新创建的 itemConfig
                    SerializedProperty arrayElement = mItemConfigs.GetArrayElementAtIndex(mItemConfigs.arraySize - 1);
                    arrayElement.objectReferenceValue = itemConfig;

                    // 保存所有更改到资产
                    AssetDatabase.SaveAssets();
                    // 刷新资源
                    AssetDatabase.Refresh();
                }

                // 遍历 ItemConfigs 列表，为每个配置绘制编辑器界面
                for (int i = 0; i < mItemConfigs.arraySize; i++)
                {
                    // 获取列表中当前配置的序列化属性
                    SerializedProperty itemConfig = mItemConfigs.GetArrayElementAtIndex(i);
                    GUILayout.BeginHorizontal();
                    // 绘制当前配置的属性编辑器界面
                    // .DrawProperty() 是 QFramework 提供的，用于绘制 Property
                    itemConfig.DrawProperty();

                    // 添加一个弹性空间
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("X"))
                    {
                        // 获取当前索引 i 处的 ItemConfig 对象的序列化属性
                        SerializedProperty arrayElement = mItemConfigs.GetArrayElementAtIndex(i);
                        // 从 AssetDatabase 中移除该对象，这会删除它作为子资产的关联
                        AssetDatabase.RemoveObjectFromAsset(arrayElement.objectReferenceValue);
                        // 删除 mItemConfigs 列表中索引 i 处的元素
                        mItemConfigs.DeleteArrayElementAtIndex(i);

                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                    GUILayout.EndHorizontal();
                }

                if (GUILayout.Button("Create Code"))
                {
                    // 获取当前编辑的 ItemDatabase 实例
                    // target 指的是次编辑器对应的脚本，也就是 ItemDatabase
                    var itemDatabase = target as ItemDatabase;

                    // 获取当前 ItemDatabase 脚本的文件路径，并确定生成代码的保存位置
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
                        .Namespace(itemDatabase.NameSpace, ns =>
                        {
                            // 在命名空间中定义一个类
                            ns.Class("Items", String.Empty, false, false, c =>
                            {
                                // 为每个 itemDB.ItemConfigs 生成一个静态字符串字段
                                foreach (ItemConfig itemConfig in itemDatabase.ItemConfigs)
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

                // 应用对序列化对象所做的所有修改
                serializedObject.ApplyModifiedProperties();
            }
        }
#endif
    }
}
