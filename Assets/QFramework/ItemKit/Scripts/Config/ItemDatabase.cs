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
        [DisplayLabel("命名空间：")]
        public string NameSpace = "QFramework.Example";

        public List<ItemConfig> ItemConfigs = new List<ItemConfig>();

#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(ItemDatabase))]
        public class ItemDatabaseEditor : UnityEditor.Editor
        {
            public class ItemEditorObj
            {
                public bool Foldout
                {
                    get => EditorPrefs.GetBool(ItemConfig.GetName + "_foldout", true);
                    set => EditorPrefs.SetBool(ItemConfig.GetName + "_foldout", value);
                }
                public Editor Editor = null;
                public ItemConfig ItemConfig = null;
            }

            private List<ItemEditorObj> mItemEditors = new List<ItemEditorObj>();

            // mItemConfigs 是一个序列化属性，用于编辑器中显示和编辑 ItemConfigs 列表
            private SerializedProperty mItemConfigs;

            private void OnEnable()
            {
                // 获取并存储 ItemConfigs 列表的序列化属性引用
                mItemConfigs = serializedObject.FindProperty("ItemConfigs");

                RefreshItemEditors();
            }

            private void RefreshItemEditors()
            {
                mItemEditors.Clear();

                for (int i = 0; i < mItemConfigs.arraySize; i++)
                {
                    SerializedProperty itemSO = mItemConfigs.GetArrayElementAtIndex(i);
                    Editor editor = CreateEditor(itemSO.objectReferenceValue);
                    mItemEditors.Add(new ItemEditorObj()
                    {
                        Editor = editor,
                        ItemConfig = itemSO.objectReferenceValue as ItemConfig,
                    });
                }
            }

            private string mSearchKey = string.Empty;
            // 使用 QFramework 提供的字体设置封装
            FluentGUIStyle mHeader = FluentGUIStyle.Label().FontBold();

            Queue<Action> mActionQueue = new Queue<Action>();

            // OnInspectorGUI 重写自定义编辑器的GUI布局和行为
            public override void OnInspectorGUI()
            {
                //base.OnInspectorGUI();

                // serializedObject 是 ItemDatabase 的可序列化的对象
                // .Update() 更新序列化对象的状态，以准备显示
                serializedObject.Update();

                GUILayout.BeginVertical("box");

                // 绘制除 ItemConfigs 之外的所有属性
                // .DrawProperties() 是 QFramework 提供的，其中包含 base.OnInspectorGUI() 的功能
                // .DrawProperties(是否绘制脚本，缩进值，忽略)
                serializedObject.DrawProperties(true, 0, "ItemConfigs");

                if (mItemConfigs.arraySize != mItemEditors.Count)
                {
                    RefreshItemEditors();
                }

                if (GUILayout.Button("生成代码"))
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
                                    c.Custom($"public static string {itemConfig.Key} = \"{itemConfig.Key}\";");
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
                GUILayout.EndVertical();

                // 加个分隔符
                EditorGUILayout.Separator();

                GUILayout.Label("物品列表：", mHeader);
                GUILayout.BeginHorizontal();
                GUILayout.Label("搜索：", GUILayout.Width(40));
                mSearchKey = EditorGUILayout.TextField(mSearchKey);
                GUILayout.EndHorizontal();

                // 遍历 ItemConfigs 列表，为每个配置绘制编辑器界面
                for (int i = 0; i < mItemEditors.Count; i++)
                {
                    // 获取列表中当前配置的序列化属性
                    //SerializedProperty itemConfig = mItemConfigs.GetArrayElementAtIndex(i);

                    ItemEditorObj itemEditor = mItemEditors[i];

                    if (!itemEditor.ItemConfig.Name.Contains(mSearchKey) && !itemEditor.ItemConfig.Key.Contains(mSearchKey))
                    {
                        continue;
                    }

                    // 绘制背景框
                    GUILayout.BeginVertical("box");
                    GUILayout.BeginHorizontal();
                    // 创建一个默认展开的可折叠区域，标题为 ItemConfig 的名称
                    itemEditor.Foldout = EditorGUILayout.Foldout(itemEditor.Foldout, itemEditor.ItemConfig.Name);

                    // 添加一个弹性空间
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("X"))
                    {
                        int index = i;

                        // 弹窗提示
                        if (EditorUtility.DisplayDialog("删除物品", "确定要删除吗？\n（此操作不可恢复）", "删除", "取消"))
                        {
                            mActionQueue.Enqueue(() =>
                            {
                                // 获取当前索引 i 处的 ItemConfig 对象的序列化属性
                                SerializedProperty arrayElement = mItemConfigs.GetArrayElementAtIndex(index);
                                // 从 AssetDatabase 中移除该对象，这会删除它作为子资源的关联
                                AssetDatabase.RemoveObjectFromAsset(arrayElement.objectReferenceValue);
                                // 删除 mItemConfigs 列表中索引 i 处的元素
                                mItemConfigs.DeleteArrayElementAtIndex(index);

                                AssetDatabase.SaveAssets();
                                AssetDatabase.Refresh();
                            });
                        }
                    }
                    GUILayout.EndHorizontal();

                    if (itemEditor.Foldout)
                    {
                        // 当前绘制方式还比较消耗性能，后续还需要优化
                        //Editor.CreateEditor(itemObj).OnInspectorGUI();

                        itemEditor.Editor.OnInspectorGUI();
                    }
                    GUILayout.EndVertical();
                }

                if (GUILayout.Button("添加物品"))
                {
                    mActionQueue.Enqueue(() =>
                    {
                        // 创建一个新的 ItemConfig 实例
                        ItemConfig itemConfig = ItemConfig.CreateInstance<ItemConfig>();
                        itemConfig.name = nameof(ItemConfig);
                        itemConfig.Name = "新物品";
                        itemConfig.Key = "item_new";

                        // 将新创建的 itemConfig 添加到 ItemDatabase 的资源中
                        AssetDatabase.AddObjectToAsset(itemConfig, target);
                        // 在 ItemConfigs 列表中添加一个新的元素
                        mItemConfigs.InsertArrayElementAtIndex(mItemConfigs.arraySize);

                        // 获取新添加的元素的序列化属性，并设置其值为新创建的 itemConfig
                        SerializedProperty arrayElement = mItemConfigs.GetArrayElementAtIndex(mItemConfigs.arraySize - 1);
                        arrayElement.objectReferenceValue = itemConfig;

                        serializedObject.ApplyModifiedPropertiesWithoutUndo();

                        // 保存所有更改到资源
                        AssetDatabase.SaveAssets();
                        // 刷新资源
                        AssetDatabase.Refresh();
                    });
                }

                if (mActionQueue.Count > 0)
                {
                    mActionQueue.Dequeue().Invoke();
                }

                // 应用对序列化对象所做的所有修改，不支持撤销
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }
#endif
    }
}
