using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public class ItemKit
    {
        public static Item Item1 = new Item("item_1", "��Ʒ1");
        public static Item Item2 = new Item("item_2", "��Ʒ2");
        public static Item Item3 = new Item("item_3", "��Ʒ3");
        public static Item Item4 = new Item("item_4", "��Ʒ4");
        public static Item Item5 = new Item("item_5", "��Ʒ5");

        public static List<Slot> Slots = new List<Slot>()
        {
            new Slot(ItemKit.Item1, 1),
            new Slot(ItemKit.Item2, 2),
            new Slot(ItemKit.Item3, 3),
            new Slot(ItemKit.Item4, 4),
        };

        public static Dictionary<string, Item> ItemByKey = new Dictionary<string, Item>()
        {
            { ItemKit.Item1.Key, ItemKit.Item1 },
            { ItemKit.Item2.Key, ItemKit.Item2 },
            { ItemKit.Item3.Key, ItemKit.Item3 },
            { ItemKit.Item4.Key, ItemKit.Item4 },
        };

        /// <summary>
        /// ������Ʒ�ļ�ֵ���Ҷ�Ӧ�Ĳ�λ���ò�λ�����Ѿ�������Ʒ����Ʒ��������0��
        /// </summary>
        /// <param name="itemKey">��Ʒ�ļ�ֵ��</param>
        /// <returns>�����ҵ��Ĳ�λ�����û���ҵ����������Ĳ�λ�򷵻�null��</returns>
        public static Slot FindSlotByKey(string itemKey)
        {
            Slot slot = ItemKit.Slots.Find(s => s.Item != null && s.Count > 0 && s.Item.Key == itemKey);
            return slot;
        }

        /// <summary>
        /// ����һ���յĲ�λ����û�а����κ���Ʒ�Ĳ�λ��
        /// </summary>
        /// <returns>�����ҵ��Ŀղ�λ�����û�пղ�λ�򷵻�null��</returns>
        public static Slot FindEmptySlot()
        {
            return ItemKit.Slots.Find(s => s.Count == 0);
        }

        /// <summary>
        /// ����һ���������ָ����Ʒ�Ĳ�λ�����Ȳ����Ѱ�������Ʒ�Ĳ�λ�����û������ҿղ�λ��
        /// </summary>
        /// <param name="itemKey">Ҫ��ӵ���Ʒ�ļ�ֵ��</param>
        /// <returns>���ؿ������Ʒ�Ĳ�λ�����û�к��ʵĲ�λ�򷵻�null��</returns>
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

            if (slot == null)
            {
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

            slot.Count -= removeCount;

            if (slot.Count == 0)
            {
                // ����������0ʱ�������λ�е���Ʒ����
                slot.Item = null;
            }
        }
    }
}
