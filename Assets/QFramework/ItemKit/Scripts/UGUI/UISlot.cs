using QFramework.Example;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace QFramework
{
    public class UISlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public Image Icon;
        public Text Count;

        public Slot Data { get; private set; }

        private bool mDragging = false;

        public UISlot InitWithData(Slot data)
        {
            Data = data;

            void UpdateView()
            {
                if (Data.Count == 0)
                {
                    Icon.Hide();
                    Count.text = "";
                }
                else
                {
                    Icon.Show();
                    if (data.Item != null)
                    {
                        if (data.Item.GetIcon)
                            Icon.sprite = data.Item.GetIcon;
                    }
                    Count.text = Data.Count.ToString();
                }
            }

            Data.Changed.Register(() =>
            {
                UpdateView();

            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            UpdateView();

            return this;    // ��������������ʽ����
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (mDragging || Data.Count < 1) return;
            mDragging = true;

            // Ϊ�˽����ק��Ʒʱ�㼶����
            Canvas canvas = Icon.gameObject.GetOrAddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 1000;

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
                Canvas canvas = Icon.GetComponent<Canvas>();
                canvas.DestroySelf();

                // λ�û�ԭ��������㣩
                Icon.LocalPositionIdentity();

                if(ItemKit.CurrentSlotPointerOn)
                {
                    UISlot uiSlot = ItemKit.CurrentSlotPointerOn;
                    RectTransform rectTrans = uiSlot.transform as RectTransform;
                    if (RectTransformUtility.RectangleContainsScreenPoint(rectTrans, Input.mousePosition))
                    {
                        // ��Ʒ����
                        if (Data.Count > 0)
                        {
                            // ����Ŀ����λ����
                            IItem itemCache = uiSlot.Data.Item;
                            int countCache = uiSlot.Data.Count;

                            // ����ǰ��λ���ݱ��浽Ŀ����λ��
                            uiSlot.Data.Item = Data.Item;
                            uiSlot.Data.Count = Data.Count;

                            // ��Ŀ����λ���ݱ��浽��ǰ��λ��
                            Data.Item = itemCache;
                            Data.Count = countCache;

                            uiSlot.Data.Changed.Trigger();
                            Data.Changed.Trigger();
                        }
                    }
                }
                else
                {
                    Data.Item = null;
                    Data.Count = 0;

                    Data.Changed.Trigger();
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
                Icon.LocalPosition2D(localPos);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            ItemKit.CurrentSlotPointerOn = this;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (ItemKit.CurrentSlotPointerOn == this)
            {
                ItemKit.CurrentSlotPointerOn = null;
            }
        }
    }
}
