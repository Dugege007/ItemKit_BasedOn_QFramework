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
        [HorizontalGroup("�������ͺ�ֵ")]
        [VerticalGroup("�������ͺ�ֵ/name")]
        [LabelText("����"), LabelWidth(42)]
        public string Name;

        [HideLabel]
        [HorizontalGroup("�������ͺ�ֵ")]
        [VerticalGroup("�������ͺ�ֵ/type")]
        public AttributeType Type;
    }

    [CreateAssetMenu(menuName = "@ItemKit/Create Item Database")]
    public class ItemDatabase : ScriptableObject
    {
        [LabelText("�����ռ�")]
        public string NameSpace = "QFramework.Example";
        [LabelText("����·��")]
        public string CodeGenPath = "";

#if UNITY_EDITOR
        [Button("���� Items ����", ButtonSizes.Large), GUIColor("green")]
        private void GenerateCode()
        {
            var itemDatabase = this;
            string path = CodeGenPath;

            if (CodeGenPath.IsNullOrEmpty())
            {
                CodeGenPath = EditorUtility.SaveFilePanelInProject("Items.cs", "Items", "cs", null);
            }

            // ��ȡ��ǰ ItemDatabase �ű����ļ�·������ȷ�����ɴ���ı���λ��
            string filePath = path;

            // ʹ�� QFramework �еĴ������ɹ���
            // ����һ�����������������������ɴ���ṹ
            ICodeScope rootCode = new RootCode()
                // ��������ռ�
                .Using("UnityEngine")
                .Using("QFramework")
                // ��һ��
                .EmptyLine()
                // ���������ռ�
                .Namespace(itemDatabase.NameSpace, ns =>
                {
                    // �������ռ��ж���һ����
                    ns.Class("Items", String.Empty, false, false, c =>
                    {
                        // Ϊÿ�� itemDB.ItemConfigs ����һ����̬�ַ����ֶ�
                        foreach (ItemConfig itemConfig in itemDatabase.ItemConfigs)
                        {
                            // Items ����ͼ���侲̬�ֶγ�ʼ��ʱֱ�Ӵ� ItemKit.ItemByKey �ֵ��з�����Ŀ
                            // ����Щ��Ŀ���ܻ�û�б���ӵ��ֵ���
                            // Ϊ�˸���ݵؽ����һ���⣬����ʱ�����ӳټ���
                            c.Custom($"public static IItem {itemConfig.Key} => ItemKit.ItemByKey[\"{itemConfig.Key}\"];");
                            c.Custom($"public static string {itemConfig.Key}_key = \"{itemConfig.Key}\";\n");

                            Debug.Log(itemConfig.Key);
                        }
                    });
                });

            // �����򸲸��ļ�����׼��д�����ɵĴ���
            // ʹ�� using ����Զ����� StreamWriter ���������ڡ�
            // ���뿪 using ������������ʱ��fileWriter �� Dispose �����ᱻ�Զ����ã�ȷ���ļ���Դ����ȷ�رա�
            using StreamWriter fileWriter = File.CreateText(filePath);
            // ����һ������д��������������������ת��Ϊ�ַ���
            FileCodeWriter codeWriter = new FileCodeWriter(fileWriter);
            // ���ɴ��벢д���ļ�
            rootCode.Gen(codeWriter);

            // ��������δ�������Դ����
            AssetDatabase.SaveAssets();
            // ˢ�� Unity �༭������Դ���ݿ�
            AssetDatabase.Refresh();
        }
#endif

        [LabelText("���Զ���"), VerticalGroup("����", Order = 10, AnimateVisibility = true)]
        public List<ItemAttributeDefine> AttributesDefine = new List<ItemAttributeDefine>();

        [Searchable, VerticalGroup("����", AnimateVisibility = true)]
        [TableList(ShowIndexLabels = true)]
        public List<ItemConfig> ItemConfigs = new List<ItemConfig>();

#if UNITY_EDITOR
        [Button("��� ItemConfig", ButtonSizes.Large), GUIColor("yellow"), VerticalGroup("����")]
        private void AddItemConfig()
        {
            // ����һ���µ� ItemConfig ʵ��
            ItemConfig itemConfigSO = CreateInstance<ItemConfig>();
            itemConfigSO.ItemDatabase = this;
            itemConfigSO.name = nameof(ItemConfig);
            itemConfigSO.Name = "����Ʒ";
            itemConfigSO.Key = "item_new";

            // ���´����� itemConfig ��ӵ� ItemConfigGroup ����Դ��
            AssetDatabase.AddObjectToAsset(itemConfigSO, this);
            // �� ItemConfigs �б������һ���µ�Ԫ��
            ItemConfigs.Add(itemConfigSO);

            Debug.Log($"�����Ʒ����: {itemConfigSO.GetKey}");
            // �������и��ĵ���Դ
            AssetDatabase.SaveAssets();
            // ˢ����Դ
            AssetDatabase.Refresh();
        }

        public void DuplicateItemConfig(int index, ItemConfig itemConfig)
        {
            // ����һ���µ� ItemConfig ʵ��
            ItemConfig itemConfigSO = CreateInstance<ItemConfig>();
            itemConfigSO.ItemDatabase = this;
            itemConfigSO.name = itemConfig.Key;
            itemConfigSO.Name = string.Empty;
            itemConfigSO.Key = "item_new";
            itemConfigSO.Attributes = new List<ItemAttribute>();
            itemConfigSO.IsStackable = itemConfig.IsStackable;
            itemConfigSO.HasMaxStackableCount = itemConfig.HasMaxStackableCount;
            itemConfigSO.MaxStackableCount = itemConfig.MaxStackableCount;

            // ���´����� itemConfig ��ӵ� ItemConfigGroup ����Դ�ļ���
            AssetDatabase.AddObjectToAsset(itemConfigSO, this);
            // �� ItemConfigs �б������һ���µ�Ԫ��
            ItemConfigs.Insert(index + 1, itemConfigSO);

            // �������и��ĵ���Դ
            AssetDatabase.SaveAssets();
            // ˢ����Դ
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

                        // ͬ��
                        if (attribute == null)
                        {
                            item.Attributes.Add(new ItemAttribute()
                            {
                                Name = attributeDefine.Name,
                                Type = attributeDefine.Type,
                            });

                            item.Attributes.Add(attribute);
                        }
                        else
                        {
                            attribute.Type = attributeDefine.Type;
                        }

                        // ȥ������
                        if (item.Attributes.Count != AttributesDefine.Count)
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