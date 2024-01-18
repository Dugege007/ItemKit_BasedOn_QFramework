using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public class UISlotGroup : MonoBehaviour
    {
        public string GroupKey;
        public UISlot UISlotTemplate;
        public RectTransform UISlotRoot;

        private void Start()
        {
            UISlotTemplate.Hide();

            Refresh();
        }

        public void Refresh()
        {
            UISlotRoot.DestroyChildren();

            foreach (var slot in ItemKit.GetSlotGroupByKey(GroupKey).Slots)
            {
                UISlotTemplate.InstantiateWithParent(UISlotRoot)
                    .InitWithData(slot)
                    .Show();
            }
        }
    }
}
