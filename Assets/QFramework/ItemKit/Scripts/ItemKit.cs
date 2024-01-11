using QFramework.Example;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public class ItemKit
    {
        public static List<Slot> Slots = new List<Slot>();
        public static Dictionary<string, IItem> ItemByKey = new Dictionary<string, IItem>();

        /// <summary>
        /// ͨ�� Resources ����ָ������Ʒ�����ļ���
        /// </summary>
        /// <param name="itemConfigFileName">��Ʒ�����ļ������ƣ�������·������չ����</param>
        public static void LoadItemConfigByResources(string itemConfigFileName)
        {
            // ʹ�� Resources.Load ������Ϊ itemConfigFileName �� ItemConfig ����
            AddItemConfig(Resources.Load<ItemConfig>(itemConfigFileName));
        }

        /// <summary>
        /// ����Ʒ������ӵ���Ʒ�ֵ��С�
        /// </summary>
        /// <param name="itemConfig">ʵ���� IItem �ӿڵ���Ʒ���ö���</param>
        public static void AddItemConfig(IItem itemConfig)
        {
            ItemByKey.Add(itemConfig.GetKey, itemConfig);
        }

        /// <summary>
        /// ����һ���µ���λ����ӵ���λ�б��С�
        /// </summary>
        /// <param name="item">Ҫ������λ����Ʒ����Ĭ��Ϊnull��</param>
        /// <param name="count">��Ʒ��������Ĭ��Ϊ0��</param>
        public static void CreateSlot(IItem item = null, int count = 0)
        {
            Slots.Add(new Slot(item, count));
        }

        /// <summary>
        /// ������Ʒ�ļ�ֵ���Ҷ�Ӧ����λ������λ�����Ѿ�������Ʒ����Ʒ��������0��
        /// </summary>
        /// <param name="itemKey">��Ʒ�ļ�ֵ��</param>
        /// <returns>�����ҵ�����λ�����û���ҵ�������������λ�򷵻�null��</returns>
        public static Slot FindSlotByKey(string itemKey)
        {
            Slot slot = ItemKit.Slots.Find(s => s.Item != null && s.Count > 0 && s.Item.GetKey == itemKey);
            return slot;
        }

        /// <summary>
        /// ����һ���յ���λ����û�а����κ���Ʒ����λ��
        /// </summary>
        /// <returns>�����ҵ��Ŀ���λ�����û�п���λ�򷵻�null��</returns>
        public static Slot FindEmptySlot()
        {
            return ItemKit.Slots.Find(s => s.Count == 0);
        }

        /// <summary>
        /// ����һ���������ָ����Ʒ����λ�����Ȳ����Ѱ�������Ʒ����λ�����û������ҿ���λ��
        /// </summary>
        /// <param name="itemKey">Ҫ��ӵ���Ʒ�ļ�ֵ��</param>
        /// <returns>���ؿ������Ʒ����λ�����û�к��ʵ���λ�򷵻�null��</returns>
        public static Slot FindAddableSlot(string itemKey)
        {
            Slot slot = FindSlotByKey(itemKey);

            if (slot == null)
            {
                slot = FindEmptySlot();
                if (slot != null)
                    slot.Item = ItemKit.ItemByKey[itemKey];
            }

            return slot;
        }

        /// <summary>
        /// �򱳰������ָ����ֵ����Ʒ��
        /// </summary>
        /// <param name="itemKey">Ҫ��ӵ���Ʒ�ļ�ֵ��</param>
        /// <param name="addCount">Ҫ��ӵ���Ʒ������Ĭ��Ϊ1��</param>
        public static void AddItem(string itemKey, int addCount = 1)
        {
            Slot slot = FindAddableSlot(itemKey);

            if (slot == null || slot.Count >= 99)
            {
                slot.Count = 99;
                Debug.Log("��������");
                return;
            }

            slot.Count += addCount;
        }

        /// <summary>
        /// �ӱ������Ƴ�ָ����ֵ����Ʒ��
        /// </summary>
        /// <param name="itemKey">Ҫ�Ƴ�����Ʒ�ļ�ֵ��</param>
        /// <param name="removeCount">Ҫ�Ƴ�����Ʒ������Ĭ��Ϊ1��</param>
        public static void RemoveItem(string itemKey, int removeCount = 1)
        {
            Slot slot = FindSlotByKey(itemKey);

            if (slot == null || slot.Count < 1)
            {
                Debug.Log("������û�д���Ʒ");
                return;
            }

            if (slot.Count < removeCount)
            {
                Debug.Log("��Ʒ����");
                return;
            }

            slot.Count -= removeCount;

            if (slot.Count <= 0)
            {
                slot.Count = 0;
                // ����������0ʱ�������λ�е���Ʒ����
                slot.Item = null;
            }
        }
    }
}
