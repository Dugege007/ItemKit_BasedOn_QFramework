using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEditor;

namespace QFramework
{
    [CreateAssetMenu(menuName = "@ItemKit/Create Item ConfigGroup")]
    public class ItemConfigGroup : ScriptableObject
    {
        public string NameSpace = "QFramework.Example";

        [Searchable]
        [TableList(ShowIndexLabels = true)]
        public List<ItemConfig> ItemConfigs = new List<ItemConfig>();

        [Button("���� ItemConfig", ButtonSizes.Large), GUIColor("yellow")]
        private void AddItemConfig()
        {
            // ����һ���µ� ItemConfig ʵ��
            ItemConfig itemConfig = CreateInstance<ItemConfig>();
            itemConfig.ItemConfigGroup = this;
            itemConfig.name = nameof(ItemConfig);
            itemConfig.Name = "����Ʒ";
            itemConfig.Key = "item_new";

            // ���´����� itemConfig ���ӵ� ItemConfigGroup ����Դ��
            AssetDatabase.AddObjectToAsset(itemConfig, this);
            // �� ItemConfigs �б�������һ���µ�Ԫ��
            ItemConfigs.Add(itemConfig);

            // �������и��ĵ���Դ
            AssetDatabase.SaveAssets();
            // ˢ����Դ
            AssetDatabase.Refresh();
        }

        public void DuplicateItemConfig(int index, ItemConfig itemConfig)
        {
            // ����һ���µ� ItemConfig ʵ��
            ItemConfig itemConfigSO = CreateInstance<ItemConfig>();
            itemConfigSO.ItemConfigGroup = this;
            itemConfigSO.name = itemConfig.Key;
            itemConfigSO.Name = string.Empty;
            itemConfigSO.Key = "item_new";
            itemConfigSO.IsWeapon = itemConfig.IsWeapon;
            itemConfigSO.IsStackable = itemConfig.IsStackable;
            itemConfigSO.HasMaxStackableCount = itemConfig.HasMaxStackableCount;
            itemConfigSO.MaxStackableCount = itemConfig.MaxStackableCount;

            // ���´����� itemConfig ���ӵ� ItemConfigGroup ����Դ�ļ���
            AssetDatabase.AddObjectToAsset(itemConfigSO, this);
            // �� ItemConfigs �б�������һ���µ�Ԫ��
            ItemConfigs.Insert(index + 1, itemConfigSO);

            // �������и��ĵ���Դ
            AssetDatabase.SaveAssets();
            // ˢ����Դ
            AssetDatabase.Refresh();
        }

        [Button("���� Items ����", ButtonSizes.Large), GUIColor("green")]
        private void GenerateCode()
        {
            var itemDatabase = this;
            // ��ȡ��ǰ ItemDatabase �ű����ļ�·������ȷ�����ɴ���ı���λ��
            string filePath = AssetDatabase.GetAssetPath(itemDatabase).GetFolderPath() + "/Items.cs";

            // ʹ�� QFramework �еĴ������ɹ���
            // ����һ�����������������������ɴ���ṹ
            ICodeScope rootCode = new RootCode()
                // ���������ռ�
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
                            c.Custom($"public static string {itemConfig.Key} = \"{itemConfig.Key}\";");
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