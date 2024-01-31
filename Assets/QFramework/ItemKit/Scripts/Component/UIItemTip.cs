using System.Collections;
using System.Collections.Generic;
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

        public static void Show(UISlot slot)
        {
            if (slot.Data.Item != null)
            {
                mDefault.Icon.sprite = slot.Data.Item.GetIcon;
                mDefault.NameText.text = slot.Data.Item.GetName;
                mDefault.DescriptionText.text = slot.Data.Item.GetDescription;

                mDefault.TipPanel.Show();
            }
        }

        public static void Hide()
        {
            mDefault.TipPanel.Hide();
        }

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

        private void OnDestroy()
        {
            mDefault = null;
        }
    }
}
