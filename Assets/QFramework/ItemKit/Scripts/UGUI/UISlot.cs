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

        public Slot Data { get; private set; }

        private bool mDragging = false;

        public UISlot InitWithData(Slot data)
        {
            Data = data;

            if (Data.Count == 0)
            {
                Name.text = "��";
                Count.text = "";
            }
            else
            {
                Name.text = Data.Item.Name;
                Count.text = Data.Count.ToString();
            }

            return this;    // ��������������ʽ����
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (mDragging || Data.Count < 1) return;
            mDragging = true;

            UGUIInventoryExample controller = FindAnyObjectByType<UGUIInventoryExample>();
            Name.Parent(controller);
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
                Name.Parent(transform);
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

                        // ��Ʒ����
                        if (Data.Count > 0)
                        {
                            // ����Ŀ����λ����
                            Item itemCache = uiSlot.Data.Item;
                            int countCache = uiSlot.Data.Count;

                            // ����ǰ��λ���ݱ��浽Ŀ����λ��
                            uiSlot.Data.Item = Data.Item;
                            uiSlot.Data.Count = Data.Count;

                            // ��Ŀ����λ���ݱ��浽��ǰ��λ��
                            Data.Item = itemCache;
                            Data.Count = countCache;

                            // ˢ��
                            FindAnyObjectByType<UGUIInventoryExample>().Refresh();
                        }

                        break;
                    }
                }

                if (throwItem)
                {
                    Data.Item = null;
                    Data.Count = 0;
                    // ˢ��
                    FindAnyObjectByType<UGUIInventoryExample>().Refresh();
                }
            }

            mDragging = false;
        }

        // ͬ����Ʒ�����λ�ã���קʱ��
        private void SyncItemToMousePos()
        {
            Vector3 mousePos = Input.mousePosition;
            UGUIInventoryExample controller = FindAnyObjectByType<UGUIInventoryExample>();
            // ����Ļ�ϵĵ�ת��Ϊλ��ָ�� RectTransform ƽ���ϵı�������ϵ�е�λ��
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                controller.transform as RectTransform, // rect��Ŀ�� RectTransform������Ļ��ת������� RectTransform �����ƽ����
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
