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
                mDefault.Icon.sprite = slot.Data.Item.GetIcon;
                mDefault.NameText.text = slot.Data.Item.GetName;
                mDefault.DescriptionText.text = slot.Data.Item.GetDescription;

                UpdatePosition();

                mDefault.TipPanel.Show();
            }
        }

        public static void Hide()
        {
            mDefault.TipPanel.Hide();
        }

        public static void UpdatePosition()
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3[] corners = new Vector3[4];
            RectTransform rectTrans = mDefault.TipPanel.transform as RectTransform;

            // 更新TipPanel内容后，强制刷新布局
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTrans);

            // 获取界面的四个角点
            rectTrans.GetWorldCorners(corners);
            float width = corners[3].x - corners[0].x;
            float height = corners[1].y - corners[0].y;

            // 根据鼠标和屏幕的相对位置，调整生成 TipPanel 的位置
            if (mousePos.y < height && mousePos.x > width)
                rectTrans.position = mousePos + 0.51f * height * Vector3.up + 0.51f * width * Vector3.left;
            else if (mousePos.y > height && mousePos.x < width)
                rectTrans.position = mousePos + 0.51f * height * Vector3.down + 0.51f * width * Vector3.right;
            else if (mousePos.y < height && mousePos.x < width)
                rectTrans.position = mousePos + 0.51f * height * Vector3.up + 0.51f * width * Vector3.right;
            else
                rectTrans.position = mousePos + 0.51f * height * Vector3.down + 0.51f * width * Vector3.left;
        }

        private void OnDestroy()
        {
            mDefault = null;
        }
    }
}
