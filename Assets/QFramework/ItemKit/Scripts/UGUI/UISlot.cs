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
                // λ�û�ԭ��������㣩
                Name.LocalPositionIdentity();
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
