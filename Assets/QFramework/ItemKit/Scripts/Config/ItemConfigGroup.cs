using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEditor;
using static UnityEngine.GraphicsBuffer;

namespace QFramework
{
    [CreateAssetMenu(menuName = "@ItemKit/Create Item ConfigGroup")]
    public class ItemConfigGroup : ScriptableObject
    {
        public string NameSpace = "QFramework.Example";

        [TableList(ShowIndexLabels = true)]
        public List<ItemConfig> ItemConfigs = new List<ItemConfig>();

        private Queue<Action> mActionQueue = new Queue<Action>();

        [Button(ButtonSizes.Large)]
        private void CreateCode()
        {
            var itemDatabase = this;

            // ��ȡ��ǰ ItemDatabase �ű����ļ�·������ȷ�����ɴ���ı���λ��
            string filePath = AssetDatabase.GetAssetPath(itemDatabase).GetFolderPath() + "/Items.cs";

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

        [Button(ButtonSizes.Large)]
        private void CreateItemConfig()
        {
            // ����һ���µ� ItemConfig ʵ��
            ItemConfig itemConfig = CreateInstance<ItemConfig>();
            itemConfig.name = nameof(ItemConfig);
            itemConfig.Name = "����Ʒ";
            itemConfig.Key = "item_new";

            // ���´����� itemConfig ��ӵ� ItemDatabase ����Դ��
            AssetDatabase.AddObjectToAsset(itemConfig, this);
            // �� ItemConfigs �б������һ���µ�Ԫ��
            ItemConfigs.Add(itemConfig);

            // �������и��ĵ���Դ
            AssetDatabase.SaveAssets();
            // ˢ����Դ
            AssetDatabase.Refresh();
        }

        private void Refresh()
        {
        }
    }

//#if UNITY_EDITOR
//    [CustomEditor(typeof(ItemConfigGroup))]
//    public class ItemConfigGroupEditor : Editor
//    {
//        public class ItemConfigEditorObj
//        {
//            public Editor Editor = null;
//            public ItemConfig ItemConfig = null;
//        }

//        private string mSearchKey = string.Empty;

//        private Queue<Action> mActionQueue = new Queue<Action>();

//    }
//#endif
}