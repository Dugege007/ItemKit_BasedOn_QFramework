using System;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public class SlotGroupInScene : MonoBehaviour
    {
        [Serializable]
        public class SlotConfig
        {
            public ItemConfig Item;
            public int Count;
        }

        public string GroupKey;
        [Header("��ʼ��λ")]
        public List<SlotConfig> InitSlots = new List<SlotConfig>();
        [DisplayLabel("������ UISlotGroup")]
        public UISlotGroup UISlotGroup;

        private void Awake()
        {
            // ����Ƿ��Ѿ�����ͬ���Ĳ�λ��
            if (ItemKit.HasSlotGroup(GroupKey) == false)
            {
                // ���������ͬ���Ĳ�λ�飬�򴴽��µĲ�λ��
                SlotGroup group = ItemKit.CreateSlotGroup(GroupKey);
                foreach (var slotConfig in InitSlots)
                {
                    group.CreateSlot(slotConfig.Item, slotConfig.Count);
                }
            }
        }

        public void Open()
        {
            UISlotGroup.RefreshWithChangeGroupKey(GroupKey);
            UISlotGroup.Show();
        }

        public void Close()
        {
            UISlotGroup.Hide();
        }
    }
}
