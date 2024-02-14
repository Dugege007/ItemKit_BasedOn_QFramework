using QFramework.Example;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace QFramework
{
    public class UISlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
    {
        public RectTransform IconHolder;
        public Image Icon;
        public Text Count;

        public Slot Data { get; private set; }

        private bool mDragging = false;

        public UISlot InitWithData(Slot data)
        {
            Data?.Changed.UnRegister(UpdateView);

            Data = data;

            Data.Changed.Register(UpdateView).UnRegisterWhenGameObjectDestroyed(gameObject);

            Data.Group.TriggerOnSlotInitWithData(this);

            UpdateView();

            return this;    // ��������������ʽ����
        }

        private void UpdateView()
        {
            if (Data.Count == 0)
            {
                IconHolder.Hide();
                Count.text = "";
            }
            else
            {
                if (Data.Item.GetStackable)
                {
                    Count.text = Data.Count.ToString();
                    Count.Show();
                }
                else
                {
                    Count.Hide();
                }

                IconHolder.Show();
                if (Data.Item != null)
                {
                    if (Data.Item.GetIcon)
                        Icon.sprite = Data.Item.GetIcon;
                }
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (mDragging || Data.Count < 1) return;

            UISlot uiSlot = ItemKit.CurrentSlotPointerOn;
            if (uiSlot != null)
            {
                if (uiSlot.Data.Group.CheckCondition(Data.Item) == false)
                    return;
            }
            mDragging = true;

            // Ϊ�˽����ק��Ʒʱ�㼶����
            Canvas canvas = IconHolder.gameObject.GetOrAddComponent<Canvas>();
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
                Canvas canvas = IconHolder.GetComponent<Canvas>();
                canvas.DestroySelf();

                // λ�û�ԭ��������㣩
                IconHolder.LocalPositionIdentity();

                if (ItemKit.CurrentSlotPointerOn)
                {
                    UISlot uiSlot = ItemKit.CurrentSlotPointerOn;
                    RectTransform rectTrans = uiSlot.transform as RectTransform;
                    if (RectTransformUtility.RectangleContainsScreenPoint(rectTrans, Input.mousePosition))
                    {
                        // ��Ʒ����
                        if (Data.Count > 0)
                        {
                            // �ܷŵ�Ŀ��λ�òŽ�����Ʒ����
                            if (uiSlot.Data.Group.CheckCondition(Data.Item))
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

                                uiSlot.Data.Group.Changed.Trigger();
                                Data.Group.Changed.Trigger();
                            }
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
                IconHolder.LocalPosition2D(localPos);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Data.Group.TriggerOnSlotPointerEnter(this);
            UIItemTip.Show(this);

            ItemKit.CurrentSlotPointerOn = this;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Data.Group.TriggerOnSlotPointerExit(this);
            UIItemTip.Hide();

            if (ItemKit.CurrentSlotPointerOn == this)
                ItemKit.CurrentSlotPointerOn = null;
        }

        public UnityEvent OnSelectEvent;
        public UnityEvent OnDeselectEvent;

        public void OnSelect(BaseEventData eventData)
        {
            Data.Group.TriggerOnSlotSelect(this);
            UIItemTip.Show(this);

            ItemKit.CurrentSlotPointerOn = this;

            OnSelectEvent.Invoke();
            Debug.Log("��ѡ��");
        }

        public void OnDeselect(BaseEventData eventData)
        {
            Data.Group.TriggerOnSlotDeselect(this);
            UIItemTip.Hide();

            if (ItemKit.CurrentSlotPointerOn == this)
                ItemKit.CurrentSlotPointerOn = null;

            OnDeselectEvent.Invoke();
            Debug.Log("δѡ��");
        }
    }
}
