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
            // ʹ�� AssetDatabase �������е� ItemConfig ��Դ
            ItemConfig[] itemConfigs = AssetDatabase.FindAssets($"t:{nameof(ItemConfig)}")
                // ���ҵ��� GUID ת��Ϊ��Դ���ļ�·��
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                // ����ÿ���ļ�·����Ӧ�� ItemConfig ����
                .Select(assetPath => AssetDatabase.LoadAssetAtPath<ItemConfig>(assetPath))
                // �����ת��Ϊ����
                .ToArray();

            string filePath = "Assets/QFramework/ItemKit/Examples/Items.cs";

            // ʹ�� QFramework �еĴ������ɹ���
            // ����һ�����������������������ɴ���ṹ
            ICodeScope rootCode = new RootCode()
                // ��������ռ�
                .Using("UnityEngine")
                .Using("QFramework")
                // ��һ��
                .EmptyLine()
                // ���������ռ�
                .Namespace("QFramework.Example", ns =>
                {
                    // �������ռ��ж���һ����
                    ns.Class("Items", String.Empty, false, false, c =>
                    {
                        // Ϊÿ�� ItemConfig ����һ����̬�ַ����ֶ�
                        foreach (ItemConfig itemConfig in itemConfigs)
                        {
                            c.Custom($"public static string {itemConfig.name} = \"{itemConfig.Key}\";");
                            Debug.Log(itemConfig.Key);
                        }
                    });
                });

            // ����һ�� StringBuilder ���ڹ������ɵĴ����ı�
            StringBuilder stringBuilder = new StringBuilder();
            // ����һ������д��������������������ת��Ϊ�ַ���
            StringCodeWriter codeWriter = new StringCodeWriter(stringBuilder);
            // ���ɴ���
            rootCode.Gen(codeWriter);
            // �����ɵĴ���ת��Ϊ�ַ�������ӡ������̨
            stringBuilder.ToString().LogInfo();
        }
    }
}
