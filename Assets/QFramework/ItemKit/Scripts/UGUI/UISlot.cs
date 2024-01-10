using QFramework.Example;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace QFramework
{
    public class UISlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public Text Name;
        public Text Count;

        public Slot SlotData { get; private set; }

        private bool mDragging = false;

        public UISlot InitWithData(Slot slotData)
        {
            SlotData = slotData;

            if (SlotData.Count == 0)
            {
                Name.text = "";
                Count.text = "";
            }
            else
            {
                Name.text = SlotData.Item.Name;
                Count.text = SlotData.Count.ToString();
            }

            return this;    // ��������������ʽ����
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (mDragging) return;
            mDragging = true;

            SyncItemToMousePos();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (mDragging)
            {
                SyncItemToMousePos();
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (mDragging)
            {
                // λ�û�ԭ��������㣩
                Name.LocalPositionIdentity();

                // �������Ƿ�������һ�� UISlot ��
                UISlot[] uiSlots = transform.parent.GetComponentsInChildren<UISlot>();

                bool throwItem = true;

                foreach (var uiSlot in uiSlots)
                {
                    RectTransform rectTrans = uiSlot.transform as RectTransform;
                    if (RectTransformUtility.RectangleContainsScreenPoint(rectTrans, Input.mousePosition))
                    {
                        throwItem = false;
                    }
                }

                if (throwItem)
                {
                    SlotData.Item = null;
                    SlotData.Count = 0;
                    // ��ˢ��һ��
                    FindAnyObjectByType<UGUIInventoryExample>().Refresh();
                }
            }

            mDragging = false;
        }

        // ͬ����Ʒ�����λ�ã���קʱ��
        private void SyncItemToMousePos()
        {
            Vector3 mousePos = Input.mousePosition;

            // ����Ļ�ϵĵ�ת��Ϊλ��ָ�� RectTransform ƽ���ϵı�������ϵ�е�λ��
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                transform as RectTransform, // rect��Ŀ�� RectTransform������Ļ��ת������� RectTransform �����ƽ����
                mousePos,                   // screenPoint����Ļ���꣬ͨ�������λ�û���λ��
                null,                       // camera�������������������� Overlay ��������Ϊ null������ Camera �� WorldSpace ����Ӧ������Ⱦ�� UI Ԫ�ص������
                out Vector2 localPos        // localPoint�����������ת����ı�������ϵ�е�λ��
            ))
            {
                // ����Ʒ�������ƶ�����Ļ�ϵ����λ��
                Name.LocalPosition2D(localPos);
            }
        }
    }
}
