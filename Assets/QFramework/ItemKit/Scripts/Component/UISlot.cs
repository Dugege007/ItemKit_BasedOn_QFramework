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

            return this;    // 返回自身，用于链式调用
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

            // 为了解决拖拽物品时层级问题
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

                // 位置还原（坐标归零）
                IconHolder.LocalPositionIdentity();

                if (ItemKit.CurrentSlotPointerOn)
                {
                    UISlot uiSlot = ItemKit.CurrentSlotPointerOn;
                    RectTransform rectTrans = uiSlot.transform as RectTransform;
                    if (RectTransformUtility.RectangleContainsScreenPoint(rectTrans, Input.mousePosition))
                    {
                        // 物品交换
                        if (Data.Count > 0)
                        {
                            // 能放到目标位置才进行物品交换
                            if (uiSlot.Data.Group.CheckCondition(Data.Item))
                            {
                                // 缓存目标栏位数据
                                IItem itemCache = uiSlot.Data.Item;
                                int countCache = uiSlot.Data.Count;

                                // 将当前栏位数据保存到目标栏位中
                                uiSlot.Data.Item = Data.Item;
                                uiSlot.Data.Count = Data.Count;

                                // 将目标栏位数据保存到当前栏位中
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

        // 同步物品到鼠标位置（拖拽时）
        private void SyncItemToMousePos()
        {
            Vector3 mousePos = Input.mousePosition;
            // 将屏幕上的点转换为位于指定 RectTransform 平面上的本地坐标系中的位置
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                transform as RectTransform, // rect：目标 RectTransform，将屏幕点转换到这个 RectTransform 定义的平面内
                mousePos,                   // screenPoint：屏幕坐标，通常是鼠标位置或触摸位置
                null,                       // camera：相关联的摄像机，对于 Overlay 画布可以为 null，对于 Camera 和 WorldSpace 画布应该是渲染该 UI 元素的摄像机
                out Vector2 localPos        // localPoint：输出参数，转换后的本地坐标系中的位置
            ))
            {
                // 将物品的名称移动到屏幕上的鼠标位置
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
            Debug.Log("已选择");
        }

        public void OnDeselect(BaseEventData eventData)
        {
            Data.Group.TriggerOnSlotDeselect(this);
            UIItemTip.Hide();

            if (ItemKit.CurrentSlotPointerOn == this)
                ItemKit.CurrentSlotPointerOn = null;

            OnDeselectEvent.Invoke();
            Debug.Log("未选择");
        }
    }
}
