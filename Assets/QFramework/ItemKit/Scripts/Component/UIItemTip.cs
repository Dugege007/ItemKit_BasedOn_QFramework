using UnityEngine;
using UnityEngine.UI;

namespace QFramework
{
    public class UIItemTip : MonoBehaviour
    {
        public GameObject TipPanel;
        public Image Icon;
        public Text NameText;
        public Text DescriptionText;
        public Text AttributeText;
        public Text IDText;

        private static UIItemTip mDefault;

        private static UISlot mCurrentSlot = null;

        private void Awake()
        {
            mDefault = this;
        }

        private void Start()
        {
            mDefault.TipPanel.Hide();

            if (AttributeText.text == "")
                AttributeText.Hide();
            else
                AttributeText.Show();

            if (IDText.text == "")
                IDText.Hide();
            else
                IDText.Show();
        }

        private void Update()
        {
            UpdatePosition();
        }

        public static void Show(UISlot slot)
        {
            if (slot.Data.Item != null && slot.Data.Count > 0)
            {
                mCurrentSlot = slot;

                mDefault.Icon.sprite = slot.Data.Item.GetIcon;
                mDefault.NameText.text = slot.Data.Item.GetName;
                mDefault.DescriptionText.text = slot.Data.Item.GetDescription;

                UpdatePosition();

                mDefault.TipPanel.Show();
            }
            else
            {
                mCurrentSlot = null;
            }
        }

        public static void Hide()
        {
            mDefault.TipPanel.Hide();
        }

        public static void UpdatePosition()
        {
            if (mCurrentSlot != null)
            {
                Vector3[] slotCorners = new Vector3[4];
                RectTransform slotRectTrans = mCurrentSlot.transform as RectTransform;

                // 获取 TipPanel 的四个角点
                slotRectTrans.GetWorldCorners(slotCorners);
                //float slotWidth = slotCorners[3].x - slotCorners[0].x;
                float slotHeight = slotCorners[1].y - slotCorners[0].y;

                //Vector3 mousePos = Input.mousePosition;
                Vector3 mousePos = slotRectTrans.position;
                Vector3[] tipCorners = new Vector3[4];
                RectTransform tipRectTrans = mDefault.TipPanel.transform as RectTransform;

                // 更新 TipPanel 内容后，强制刷新布局
                LayoutRebuilder.ForceRebuildLayoutImmediate(tipRectTrans);

                // 获取 TipPanel 的四个角点
                tipRectTrans.GetWorldCorners(tipCorners);
                float tipWidth = tipCorners[3].x - tipCorners[0].x;
                float tipHeight = tipCorners[1].y - tipCorners[0].y;

                // 根据鼠标和屏幕的相对位置，调整生成 TipPanel 的位置
                // Slot 位置在 右下，TipPanel 生成在 左上
                if (mousePos.y < tipHeight && mousePos.x > tipWidth)
                    tipRectTrans.position = mousePos + 0.5f * tipHeight * Vector3.up + 0.5f * tipWidth * Vector3.left + 0.5f * slotHeight * Vector3.up;
                // Slot 位置在 左上，TipPanel 生成在 右下
                else if (mousePos.y > tipHeight && mousePos.x < tipWidth)
                    tipRectTrans.position = mousePos + 0.5f * tipHeight * Vector3.down + 0.5f * tipWidth * Vector3.right + 0.5f * slotHeight * Vector3.down;
                // Slot 位置在 左下，TipPanel 生成在 右上
                else if (mousePos.y < tipHeight && mousePos.x < tipWidth)
                    tipRectTrans.position = mousePos + 0.5f * tipHeight * Vector3.up + 0.5f * tipWidth * Vector3.right + 0.5f * slotHeight * Vector3.up;
                // Slot 位置在 右上，TipPanel 生成在 左下
                else
                    tipRectTrans.position = mousePos + 0.5f * tipHeight * Vector3.down + 0.5f * tipWidth * Vector3.left + 0.5f * slotHeight * Vector3.down;
            }
        }

        private void OnDestroy()
        {
            mDefault = null;
        }
    }
}
