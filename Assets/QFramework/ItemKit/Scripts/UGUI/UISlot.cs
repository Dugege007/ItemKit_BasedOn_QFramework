using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace QFramework
{
    public class UISlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public Text Name;
        public Text Count;

        private bool mDragging = false;

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
                // 位置还原（坐标归零）
                Name.LocalPositionIdentity();
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
                Name.LocalPosition2D(localPos);
            }
        }
    }
}
